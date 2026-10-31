using Inventory.Application.Inventories.Command.RegisterInventoryIn;
using Inventory.Domain.Inventories.Entity;
using Inventory.Domain.InventoryMovements.Entity;
using Xunit;

namespace Inventory.Tests.Application.Inventories;

public class RegisterInventoryInCommandHandlerTests
{
    [Fact]
    public async Task Handle_HappyPath_IncreasesQuantityAndInsertsInMovement()
    {
        var inventoryRepository = new FakeInventoryRepository();
        var movementRepository = new FakeInventoryMovementRepository();
        var inventory = InventoryEntity.Create(productId: 1, quantity: 10);
        inventoryRepository.Seed(inventory);

        var handler = new RegisterInventoryInCommandHandler(
            inventoryRepository, movementRepository, new FakeProductRepository(), new FakeCategoryRepository(), new FakeUnitOfWork());
        var command = new RegisterInventoryInCommand { InventoryId = inventory.Id, Quantity = 5, Description = "entrada de prueba" };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(15, result.Value!.Quantity);
        Assert.Equal(15, inventoryRepository.CurrentQuantity(inventory.Id));

        var movement = Assert.Single(movementRepository.Movements);
        Assert.Equal(InventoryMovementEntity.MovementTypeIn, movement.MovementType);
        Assert.Equal(5, movement.Quantity);
    }

    [Fact]
    public async Task Handle_InventoryNotFound_ReturnsNotFound()
    {
        var inventoryRepository = new FakeInventoryRepository();
        var movementRepository = new FakeInventoryMovementRepository();

        var handler = new RegisterInventoryInCommandHandler(
            inventoryRepository, movementRepository, new FakeProductRepository(), new FakeCategoryRepository(), new FakeUnitOfWork());
        var command = new RegisterInventoryInCommand { InventoryId = 999, Quantity = 5 };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("INVENTORY_NOT_FOUND", result.Error!.Code);
        Assert.Empty(movementRepository.Movements);
    }
}
