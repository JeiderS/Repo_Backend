using Inventory.Application.InventoryMovements.Dto;
using Inventory.Domain.Inventories.Repositories;
using Inventory.Domain.InventoryMovements.Entity;
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

        // Saldo: todo cambio de stock queda como movimiento, así que se parte de la
        // cantidad actual y se deshace cada movimiento del más nuevo al más viejo.
        var balances = inventories.ToDictionary(i => i.Id, i => i.Quantity);
        var result = new List<InventoryMovementDto>();

        foreach (var movement in movements
            .Where(m => request.InventoryId is null || m.InventoryId == request.InventoryId)
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id))
        {
            inventoriesById.TryGetValue(movement.InventoryId, out var inventory);
            var product = inventory is not null && productsById.TryGetValue(inventory.ProductId, out var p) ? p : null;
            var stockAfter = balances.GetValueOrDefault(movement.InventoryId);
            balances[movement.InventoryId] = movement.MovementType == InventoryMovementEntity.MovementTypeIn
                ? stockAfter - movement.Quantity
                : stockAfter + movement.Quantity;

            result.Add(new InventoryMovementDto
            {
                Id = movement.Id,
                ProductId = inventory?.ProductId ?? 0,
                ProductName = product?.Name ?? string.Empty,
                MovementType = movement.MovementType,
                Quantity = movement.Quantity,
                StockAfter = stockAfter,
                Description = movement.Description,
                CreatedAt = movement.CreatedAt,
            });
        }

        return result;
    }
}
