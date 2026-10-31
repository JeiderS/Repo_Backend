using Inventory.Application.Inventories.Command.RegisterInventoryOut;
using Inventory.Domain.Inventories.Entity;
using Inventory.Domain.InventoryMovements.Entity;
using Xunit;

namespace Inventory.Tests.Application.Inventories;

/// <summary>
/// Cubre la defensa contra sobre-venta de stock: el camino rápido
/// (inventory.Quantity &lt; request.Quantity) y, por construcción, el mismo
/// guard que usa TryDecreaseQuantityAsync contra SQL Server
/// (WHERE Quantity &gt;= amount) -- ver InventoryRepository.cs.
/// </summary>
public class RegisterInventoryOutCommandHandlerTests
{
    private static RegisterInventoryOutCommandHandler CreateHandler(
        FakeInventoryRepository inventoryRepository,
        FakeInventoryMovementRepository movementRepository) =>
        new(inventoryRepository, movementRepository, new FakeProductRepository(), new FakeCategoryRepository(), new FakeUnitOfWork());

    [Fact]
    public async Task Handle_QuantityGreaterThanStock_ReturnsInsufficientStockAndLeavesQuantityUnchanged()
    {
        var inventoryRepository = new FakeInventoryRepository();
        var movementRepository = new FakeInventoryMovementRepository();
        var inventory = InventoryEntity.Create(productId: 1, quantity: 10);
        inventoryRepository.Seed(inventory);

        var handler = CreateHandler(inventoryRepository, movementRepository);
        var command = new RegisterInventoryOutCommand { InventoryId = inventory.Id, Quantity = 20 };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("INVENTORY_INSUFFICIENT_STOCK", result.Error!.Code);
        Assert.Equal(10, inventoryRepository.CurrentQuantity(inventory.Id));
        Assert.Empty(movementRepository.Movements);
    }

    [Fact]
    public async Task Handle_QuantityWithinStock_DecreasesQuantityAndInsertsOutMovement()
    {
        var inventoryRepository = new FakeInventoryRepository();
        var movementRepository = new FakeInventoryMovementRepository();
        var inventory = InventoryEntity.Create(productId: 1, quantity: 10);
        inventoryRepository.Seed(inventory);

        var handler = CreateHandler(inventoryRepository, movementRepository);
        var command = new RegisterInventoryOutCommand { InventoryId = inventory.Id, Quantity = 4, Description = "salida de prueba" };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(6, result.Value!.Quantity);
        Assert.Equal(6, inventoryRepository.CurrentQuantity(inventory.Id));

        var movement = Assert.Single(movementRepository.Movements);
        Assert.Equal(InventoryMovementEntity.MovementTypeOut, movement.MovementType);
        Assert.Equal(4, movement.Quantity);
    }
}
