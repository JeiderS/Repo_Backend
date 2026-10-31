using Inventory.Application.Sales.Dto;
using Inventory.Application.Sales.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Inventories.Repositories;
using Inventory.Domain.InventoryMovements.Entity;
using Inventory.Domain.InventoryMovements.Repositories;
using Inventory.Domain.Products.Repositories;
using Inventory.Domain.SaleDetails.Entity;
using Inventory.Domain.SaleDetails.Repositories;
using Inventory.Domain.Sales.Entity;
using Inventory.Domain.Sales.Repositories;
using MediatR;

namespace Inventory.Application.Sales.Command.RegisterSale;

public class RegisterSaleCommandHandler(
    ISaleRepository saleRepository,
    ISaleDetailRepository saleDetailRepository,
    IInventoryRepository inventoryRepository,
    IInventoryMovementRepository movementRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterSaleCommand, Result<SaleDetailDto, Error>>
{
    public async Task<Result<SaleDetailDto, Error>> Handle(RegisterSaleCommand request, CancellationToken cancellationToken)
    {
        // Pasada 1: valida TODAS las líneas de forma optimista sin tocar la
        // base de datos todavía -- si cualquier línea falla, es seguro
        // devolver un Result.Failure directamente (nada se mutó aún).
        var preparedLines = new List<PreparedLine>();

        foreach (var line in request.Lines)
        {
            var product = await productRepository.GetByIdAsync(line.ProductId);
            if (product is null)
                return Result<SaleDetailDto, Error>.Failure(SaleErrorBuilder.ProductNotFound());

            var inventory = await inventoryRepository.GetByProductIdAsync(line.ProductId);
            if (inventory is null)
                return Result<SaleDetailDto, Error>.Failure(SaleErrorBuilder.ProductNotFound());

            // Camino rápido: rechaza el caso común sin tocar la base de datos.
            if (inventory.Quantity < line.Quantity)
                return Result<SaleDetailDto, Error>.Failure(SaleErrorBuilder.InsufficientStock(product.Name));

            preparedLines.Add(new PreparedLine(inventory.Id, line.ProductId, product.Name, line.Quantity, product.Price));
        }

        var total = preparedLines.Sum(l => l.UnitPrice * l.Quantity);

        try
        {
            // Pasada 2: ya con todas las líneas pre-validadas, aplica la salida
            // real dentro de una transacción todo-o-nada. Si cualquier línea
            // pierde la carrera contra el stock (otra venta concurrente lo
            // consumió entre la pasada 1 y este punto), InsufficientStockException
            // revierte TODO -- incluyendo la Sale ya insertada y cualquier línea
            // anterior de esta misma pasada 2 que ya haya decrementado inventario.
            return await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var sale = SaleEntity.Create(SaleEntity.OriginManual, total);
                await saleRepository.AddAsync(sale);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var responseLines = new List<SaleDetailLineDto>();

                foreach (var line in preparedLines)
                {
                    // Defensa real contra la carrera: TryDecreaseQuantityAsync es
                    // la misma defensa atómica que usa Inventory (WHERE Quantity
                    // >= amount); la pasada 1 fue solo optimista.
                    var newQuantity = await inventoryRepository.TryDecreaseQuantityAsync(line.InventoryId, line.Quantity);
                    if (newQuantity is null)
                        throw new InsufficientStockException(line.ProductName);

                    var detail = SaleDetailEntity.Create(sale.Id, line.ProductId, line.Quantity, line.UnitPrice);
                    await saleDetailRepository.AddAsync(detail);

                    var movement = InventoryMovementEntity.Create(
                        line.InventoryId, InventoryMovementEntity.MovementTypeOut, line.Quantity, $"Venta {sale.Number}");
                    await movementRepository.AddAsync(movement);

                    responseLines.Add(new SaleDetailLineDto
                    {
                        ProductId = line.ProductId,
                        ProductName = line.ProductName,
                        Quantity = line.Quantity,
                        UnitPrice = line.UnitPrice,
                        Subtotal = detail.Subtotal,
                    });
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<SaleDetailDto, Error>.Success(new SaleDetailDto
                {
                    SaleId = sale.Id,
                    Number = sale.Number,
                    SaleDate = sale.SaleDate,
                    Origin = sale.Origin,
                    Total = sale.Total,
                    Lines = responseLines,
                });
            });
        }
        catch (InsufficientStockException ex)
        {
            // ExecuteInTransactionAsync ya revirtió todo antes de relanzar.
            return Result<SaleDetailDto, Error>.Failure(SaleErrorBuilder.InsufficientStock(ex.ProductName));
        }
    }

    private sealed record PreparedLine(int InventoryId, int ProductId, string ProductName, decimal Quantity, decimal UnitPrice);
}
