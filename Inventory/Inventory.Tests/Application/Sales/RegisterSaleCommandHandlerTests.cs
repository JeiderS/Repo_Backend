using Inventory.Application.Sales.Command.RegisterSale;
using Inventory.Domain.InventoryMovements.Entity;
using Xunit;

namespace Inventory.Tests.Application.Sales;

/// <summary>
/// Cubre el diseño de dos pasadas de RegisterSaleCommandHandler: validación
/// optimista sin mutar nada (pasada 1) y aplicación real todo-o-nada dentro
/// de una transacción (pasada 2), incluyendo el caso crítico de rollback
/// ante una carrera de stock entre ambas pasadas.
/// </summary>
public class RegisterSaleCommandHandlerTests
{
    private sealed class Harness
    {
        public FakeSalesInventoryRepository InventoryRepository { get; } = new();
        public FakeSalesProductRepository ProductRepository { get; } = new();
        public FakeSaleRepository SaleRepository { get; } = new();
        public FakeSaleDetailRepository SaleDetailRepository { get; } = new();
        public FakeSalesInventoryMovementRepository MovementRepository { get; } = new();

        public RegisterSaleCommandHandler CreateHandler()
        {
            var unitOfWork = new FakeTransactionalUnitOfWork(InventoryRepository, SaleRepository, SaleDetailRepository, MovementRepository);
            return new RegisterSaleCommandHandler(SaleRepository, SaleDetailRepository, InventoryRepository, MovementRepository, ProductRepository, unitOfWork);
        }
    }

    [Fact]
    public async Task Handle_SingleProductWithStock_RegistersSaleAndDecreasesInventory()
    {
        var harness = new Harness();
        var inventoryId = harness.InventoryRepository.Seed(productId: 1, quantity: 10);
        harness.ProductRepository.Seed(productId: 1, name: "Camiseta", price: 50m);

        var handler = harness.CreateHandler();
        var command = new RegisterSaleCommand { Lines = { new SaleLineItem { ProductId = 1, Quantity = 3 } } };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(150m, result.Value!.Total);
        Assert.Equal(7, harness.InventoryRepository.CurrentQuantity(inventoryId));

        Assert.Equal(1, harness.SaleRepository.Count);
        Assert.Equal(1, harness.SaleDetailRepository.Count);
        var movement = Assert.Single(harness.MovementRepository.Movements);
        Assert.Equal(InventoryMovementEntity.MovementTypeOut, movement.MovementType);
        Assert.Equal(3, movement.Quantity);
    }

    [Fact]
    public async Task Handle_TwoProductsBothWithStock_RegistersSaleWithTotalAsSumOfSubtotals()
    {
        var harness = new Harness();
        var inventoryId1 = harness.InventoryRepository.Seed(productId: 1, quantity: 10);
        var inventoryId2 = harness.InventoryRepository.Seed(productId: 2, quantity: 5);
        harness.ProductRepository.Seed(productId: 1, name: "Camiseta", price: 50m);
        harness.ProductRepository.Seed(productId: 2, name: "Pantalón", price: 80m);

        var handler = harness.CreateHandler();
        var command = new RegisterSaleCommand
        {
            Lines =
            {
                new SaleLineItem { ProductId = 1, Quantity = 2 },
                new SaleLineItem { ProductId = 2, Quantity = 1 },
            },
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(180m, result.Value!.Total); // (50*2) + (80*1)
        Assert.Equal(2, result.Value!.Lines.Count);
        Assert.Equal(8, harness.InventoryRepository.CurrentQuantity(inventoryId1));
        Assert.Equal(4, harness.InventoryRepository.CurrentQuantity(inventoryId2));

        Assert.Equal(1, harness.SaleRepository.Count);
        Assert.Equal(2, harness.SaleDetailRepository.Count);
        Assert.Equal(2, harness.MovementRepository.Movements.Count);
    }

    [Fact]
    public async Task Handle_SingleProductInsufficientStock_ReturnsFailureAndCreatesNothing()
    {
        var harness = new Harness();
        var inventoryId = harness.InventoryRepository.Seed(productId: 1, quantity: 2);
        harness.ProductRepository.Seed(productId: 1, name: "Camiseta", price: 50m);

        var handler = harness.CreateHandler();
        var command = new RegisterSaleCommand { Lines = { new SaleLineItem { ProductId = 1, Quantity = 5 } } };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("SALE_INSUFFICIENT_STOCK", result.Error!.Code);
        Assert.Equal(2, harness.InventoryRepository.CurrentQuantity(inventoryId));
        Assert.Equal(0, harness.SaleRepository.Count);
        Assert.Equal(0, harness.SaleDetailRepository.Count);
        Assert.Empty(harness.MovementRepository.Movements);
    }

    /// <summary>
    /// El test crítico: dos productos, el primero con stock suficiente, pero
    /// el segundo pierde una carrera de stock EXACTAMENTE entre la pasada 1
    /// (que lo valida como suficiente) y la pasada 2 (que intenta
    /// decrementarlo de verdad) -- simulado decrementando su inventario
    /// "por fuera" (otra venta concurrente) justo antes de invocar el
    /// handler, dejando el InventoryEntity ya cargado con un Quantity
    /// desactualizado (igual que el bypass real de ExecuteUpdateAsync).
    /// Prueba que el rollback revierte TODO: ni la Sale, ni el primer
    /// SaleDetail/Movement, ni el decremento ya aplicado al primer producto
    /// quedan persistidos.
    /// </summary>
    [Fact]
    public async Task Handle_SecondOfTwoProductsLosesStockRaceDuringCommit_RollsBackFirstProductDecrease()
    {
        var harness = new Harness();
        var inventoryId1 = harness.InventoryRepository.Seed(productId: 1, quantity: 10);
        var inventoryId2 = harness.InventoryRepository.Seed(productId: 2, quantity: 5);
        harness.ProductRepository.Seed(productId: 1, name: "Camiseta", price: 50m);
        harness.ProductRepository.Seed(productId: 2, name: "Pantalón", price: 80m);

        // Otra venta concurrente consume TODO el stock del producto 2 justo
        // después de que la pasada 1 de nuestra venta ya lo leyó como
        // suficiente (el InventoryEntity cacheado sigue mostrando Quantity=5).
        var stolen = await harness.InventoryRepository.TryDecreaseQuantityAsync(inventoryId2, 5);
        Assert.Equal(0, stolen);

        var handler = harness.CreateHandler();
        var command = new RegisterSaleCommand
        {
            Lines =
            {
                new SaleLineItem { ProductId = 1, Quantity = 4 },
                new SaleLineItem { ProductId = 2, Quantity = 5 },
            },
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("SALE_INSUFFICIENT_STOCK", result.Error!.Code);

        // Prueba real de rollback de punta a punta: el producto 1 (que SÍ se
        // decrementó dentro de la pasada 2, antes de que el producto 2
        // fallara) queda EXACTAMENTE en su cantidad original de 10, no en 6.
        Assert.Equal(10, harness.InventoryRepository.CurrentQuantity(inventoryId1));
        Assert.Equal(0, harness.InventoryRepository.CurrentQuantity(inventoryId2));

        Assert.Equal(0, harness.SaleRepository.Count);
        Assert.Equal(0, harness.SaleDetailRepository.Count);
        Assert.Empty(harness.MovementRepository.Movements);
    }
}
