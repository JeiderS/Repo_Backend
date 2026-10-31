using Inventory.Application.InventoryMovements.Dto;
using Inventory.Domain.Inventories.Repositories;
using Inventory.Domain.InventoryMovements.Repositories;
using Inventory.Domain.Products.Repositories;
using MediatR;

namespace Inventory.Application.InventoryMovements.Query.GetInventoryMovements;

public class GetInventoryMovementsQueryHandler(
    IInventoryMovementRepository movementRepository,
    IInventoryRepository inventoryRepository,
    IProductRepository productRepository)
    : IRequestHandler<GetInventoryMovementsQuery, IReadOnlyList<InventoryMovementDto>>
{
    public async Task<IReadOnlyList<InventoryMovementDto>> Handle(GetInventoryMovementsQuery request, CancellationToken cancellationToken)
    {
        var movements = await movementRepository.GetAllAsync();
        var inventories = await inventoryRepository.GetAllAsync();
        var products = await productRepository.GetAllAsync();

        var inventoriesById = inventories.ToDictionary(i => i.Id);
        var productsById = products.ToDictionary(p => p.Id);

        return movements
            .OrderByDescending(m => m.CreatedAt)
            .Select(movement =>
            {
                inventoriesById.TryGetValue(movement.InventoryId, out var inventory);
                var product = inventory is not null && productsById.TryGetValue(inventory.ProductId, out var p) ? p : null;

                return new InventoryMovementDto
                {
                    Id = movement.Id,
                    ProductId = inventory?.ProductId ?? 0,
                    ProductName = product?.Name ?? string.Empty,
                    MovementType = movement.MovementType,
                    Quantity = movement.Quantity,
                    Description = movement.Description,
                    CreatedAt = movement.CreatedAt,
                };
            })
            .ToList();
    }
}
