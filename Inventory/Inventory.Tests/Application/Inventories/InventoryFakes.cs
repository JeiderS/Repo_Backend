using Inventory.Domain.Categories.Entity;
using Inventory.Domain.Categories.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Inventories.Entity;
using Inventory.Domain.Inventories.Repositories;
using Inventory.Domain.InventoryMovements.Entity;
using Inventory.Domain.InventoryMovements.Repositories;
using Inventory.Domain.Products.Entity;
using Inventory.Domain.Products.Repositories;

namespace Inventory.Tests.Application.Inventories;

/// <summary>
/// Fakes en memoria (sin librería de mocking, no hay ninguna en este
/// proyecto todavía) que reproducen el comportamiento REAL de
/// InventoryRepository.TryIncreaseQuantityAsync/TryDecreaseQuantityAsync
/// (incluyendo el WHERE-guard contra stock negativo), para poder probar los
/// handlers de RegisterInventoryIn/Out sin una base SQL Server real.
///
/// IMPORTANTE: ExecuteUpdateAsync (usado por la implementación real contra
/// SQL Server) no es reproducible aquí 1:1 porque no hay un DbContext real
/// de por medio; este fake solo imita el EFECTO observable (actualización
/// atómica con guard), no el mecanismo. El código de producción sigue usando
/// ExecuteUpdateAsync contra SQL Server, sin cambios.
/// </summary>
public class FakeInventoryRepository : IInventoryRepository
{
    private readonly Dictionary<int, InventoryEntity> _inventories = new();

    // Tracked separately from the InventoryEntity instance on purpose: real
    // ExecuteUpdateAsync against SQL Server bypasses EF's change tracker, so
    // the InventoryEntity object a handler already loaded via GetByIdAsync
    // keeps showing the PRE-update Quantity even after TryIncrease/DecreaseQuantityAsync
    // commits. Handlers rely on that (they compute the post-update display
    // value as inventory.Quantity +/- request.Quantity). Mutating the entity
    // here would silently double-apply the delta and break that contract.
    private readonly Dictionary<int, decimal> _quantities = new();

    public void Seed(InventoryEntity inventory)
    {
        _inventories[inventory.Id] = inventory;
        _quantities[inventory.Id] = inventory.Quantity;
    }

    public decimal CurrentQuantity(int id) => _quantities[id];

    public Task<InventoryEntity?> GetByIdAsync(int id) =>
        Task.FromResult(_inventories.TryGetValue(id, out var inventory) ? inventory : null);

    public Task<List<InventoryEntity>> GetAllAsync() =>
        Task.FromResult(_inventories.Values.ToList());

    public Task AddAsync(InventoryEntity inventory)
    {
        _inventories[inventory.Id] = inventory;
        _quantities[inventory.Id] = inventory.Quantity;
        return Task.CompletedTask;
    }

    public Task<decimal?> TryIncreaseQuantityAsync(int inventoryId, decimal amount)
    {
        if (!_quantities.TryGetValue(inventoryId, out var quantity))
            return Task.FromResult((decimal?)null);

        var updated = quantity + amount;
        _quantities[inventoryId] = updated;
        return Task.FromResult((decimal?)updated);
    }

    public Task<decimal?> TryDecreaseQuantityAsync(int inventoryId, decimal amount)
    {
        if (!_quantities.TryGetValue(inventoryId, out var quantity) || quantity < amount)
            return Task.FromResult((decimal?)null);

        var updated = quantity - amount;
        _quantities[inventoryId] = updated;
        return Task.FromResult((decimal?)updated);
    }
}

public class FakeInventoryMovementRepository : IInventoryMovementRepository
{
    public readonly List<InventoryMovementEntity> Movements = new();

    public Task AddAsync(InventoryMovementEntity movement)
    {
        Movements.Add(movement);
        return Task.CompletedTask;
    }

    public Task<List<InventoryMovementEntity>> GetAllAsync() =>
        Task.FromResult(Movements.ToList());
}

public class FakeProductRepository : IProductRepository
{
    public Task<ProductEntity?> GetByIdAsync(int id) => Task.FromResult<ProductEntity?>(null);
    public Task<bool> ExistsAsync(int id) => Task.FromResult(false);
    public Task<IReadOnlyList<ProductEntity>> GetAllAsync() => Task.FromResult<IReadOnlyList<ProductEntity>>(new List<ProductEntity>());
    public Task AddAsync(ProductEntity product) => Task.CompletedTask;
}

public class FakeCategoryRepository : ICategoryRepository
{
    public Task<CategoryEntity?> GetByIdAsync(int id) => Task.FromResult<CategoryEntity?>(null);
    public Task<bool> ExistsAsync(int id) => Task.FromResult(false);
    public Task<IReadOnlyList<CategoryEntity>> GetAllAsync() => Task.FromResult<IReadOnlyList<CategoryEntity>>(new List<CategoryEntity>());
    public Task<bool> NameExistsAsync(string name, int? excludeCategoryId = null) => Task.FromResult(false);
    public Task AddAsync(CategoryEntity category) => Task.CompletedTask;
}

/// <summary>No hay transacción real: ejecuta la operación directamente, que es
/// lo único que los handlers necesitan para ser probados a este nivel.</summary>
public class FakeUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);

    public Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation) => operation();
}
