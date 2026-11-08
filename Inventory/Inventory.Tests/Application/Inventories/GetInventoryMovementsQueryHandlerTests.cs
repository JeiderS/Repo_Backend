using Inventory.Application.InventoryMovements.Query.GetInventoryMovements;
using Inventory.Domain.Inventories.Entity;
using Inventory.Domain.InventoryMovements.Entity;
using Xunit;

namespace Inventory.Tests.Application.Inventories;

public class GetInventoryMovementsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithInventoryId_ReturnsOnlyItsMovementsNewestFirstWithStockAfter()
    {
        var inventoryRepository = new FakeInventoryRepository();
        var movementRepository = new FakeInventoryMovementRepository();
        var inventory = InventoryEntity.Create(productId: 1, quantity: 10);
        inventoryRepository.Seed(inventory);

        // Delays keep CreatedAt strictly increasing (the fakes have no identity Id to break ties).
        await movementRepository.AddAsync(InventoryMovementEntity.Create(inventory.Id, InventoryMovementEntity.MovementTypeIn, 12, null));
        await Task.Delay(5);
        await movementRepository.AddAsync(InventoryMovementEntity.Create(99, InventoryMovementEntity.MovementTypeIn, 4, null));
        await Task.Delay(5);
        await movementRepository.AddAsync(InventoryMovementEntity.Create(inventory.Id, InventoryMovementEntity.MovementTypeOut, 9, null));

        var handler = new GetInventoryMovementsQueryHandler(movementRepository, inventoryRepository, new FakeProductRepository());

        var result = await handler.Handle(new GetInventoryMovementsQuery { InventoryId = inventory.Id }, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal((InventoryMovementEntity.MovementTypeOut, 9m, 10m), (result[0].MovementType, result[0].Quantity, result[0].StockAfter));
        Assert.Equal((InventoryMovementEntity.MovementTypeIn, 12m, 19m), (result[1].MovementType, result[1].Quantity, result[1].StockAfter));
    }

    [Fact]
    public async Task Handle_WithoutInventoryId_ReturnsAllMovements()
    {
        var inventoryRepository = new FakeInventoryRepository();
        var movementRepository = new FakeInventoryMovementRepository();
        await movementRepository.AddAsync(InventoryMovementEntity.Create(1, InventoryMovementEntity.MovementTypeIn, 3, null));
        await movementRepository.AddAsync(InventoryMovementEntity.Create(2, InventoryMovementEntity.MovementTypeIn, 4, null));

        var handler = new GetInventoryMovementsQueryHandler(movementRepository, inventoryRepository, new FakeProductRepository());

        var result = await handler.Handle(new GetInventoryMovementsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }
}
