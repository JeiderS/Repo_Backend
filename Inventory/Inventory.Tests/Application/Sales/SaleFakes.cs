using System.Reflection;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Inventories.Entity;
using Inventory.Domain.Inventories.Repositories;
using Inventory.Domain.InventoryMovements.Entity;
using Inventory.Domain.InventoryMovements.Repositories;
using Inventory.Domain.Products.Entity;
using Inventory.Domain.Products.Repositories;
using Inventory.Domain.SaleDetails.Entity;
using Inventory.Domain.SaleDetails.Repositories;
using Inventory.Domain.Sales.Entity;
using Inventory.Domain.Sales.Repositories;

namespace Inventory.Tests.Application.Sales;

/// <summary>
/// Fakes en memoria (sin librería de mocking, mismo criterio que
/// Inventory.Tests/Application/Inventories/InventoryFakes.cs) a medida para
/// probar RegisterSaleCommandHandler, incluyendo el rollback real de la
/// transacción todo-o-nada (ver FakeTransactionalUnitOfWork). Se escriben
/// fakes propios en vez de reutilizar los de Inventory porque este handler
/// necesita VARIAS filas de inventario con Id estable (uno por producto) y
/// búsqueda por ProductId, algo que InventoryFakes.cs no soporta.
/// </summary>
public class FakeSalesInventoryRepository : IInventoryRepository
{
    private readonly Dictionary<int, InventoryEntity> _byId = new();
    private readonly Dictionary<int, decimal> _quantities = new();
    private int _nextId = 1;

    /// <summary>Crea una fila de inventario para un producto y devuelve su InventoryId asignado.</summary>
    public int Seed(int productId, decimal quantity)
    {
        var id = _nextId++;
        var inventory = InventoryEntity.Create(productId, quantity);
        SetId(inventory, id);
        _byId[id] = inventory;
        _quantities[id] = quantity;
        return id;
    }

    public decimal CurrentQuantity(int inventoryId) => _quantities[inventoryId];

    public Task<InventoryEntity?> GetByIdAsync(int id) =>
        Task.FromResult(_byId.TryGetValue(id, out var inventory) ? inventory : null);

    public Task<InventoryEntity?> GetByProductIdAsync(int productId) =>
        Task.FromResult(_byId.Values.FirstOrDefault(i => i.ProductId == productId));

    public Task<List<InventoryEntity>> GetAllAsync() => Task.FromResult(_byId.Values.ToList());

    public Task AddAsync(InventoryEntity inventory) => throw new NotSupportedException("No usado por RegisterSaleCommandHandler.");

    public Task<decimal?> TryIncreaseQuantityAsync(int inventoryId, decimal amount) =>
        throw new NotSupportedException("No usado por RegisterSaleCommandHandler.");

    public Task<decimal?> TryDecreaseQuantityAsync(int inventoryId, decimal amount)
    {
        if (!_quantities.TryGetValue(inventoryId, out var quantity) || quantity < amount)
            return Task.FromResult((decimal?)null);

        var updated = quantity - amount;
        _quantities[inventoryId] = updated;
        return Task.FromResult((decimal?)updated);
    }

    /// <summary>Snapshot de las cantidades actuales, para que FakeTransactionalUnitOfWork pueda revertir.</summary>
    public Dictionary<int, decimal> SnapshotQuantities() => new(_quantities);

    public void RestoreQuantities(Dictionary<int, decimal> snapshot)
    {
        _quantities.Clear();
        foreach (var (id, quantity) in snapshot)
            _quantities[id] = quantity;
    }

    // InventoryEntity.Id tiene setter privado (lo asigna EF/SQL Server vía
    // identity al insertar); acá se asigna por reflection para poder simular
    // varias filas de inventario con Id estable y distinto dentro de un fake.
    private static void SetId(InventoryEntity inventory, int id) =>
        typeof(InventoryEntity).GetProperty(nameof(InventoryEntity.Id))!.SetValue(inventory, id);
}

public class FakeSalesProductRepository : IProductRepository
{
    private readonly Dictionary<int, ProductEntity> _products = new();

    public void Seed(int productId, string name, decimal price) =>
        _products[productId] = ProductEntity.Create(name, description: null, price, categoryId: 1, imageUrl: null);

    public Task<ProductEntity?> GetByIdAsync(int id) =>
        Task.FromResult(_products.TryGetValue(id, out var product) ? product : null);

    public Task<bool> ExistsAsync(int id) => Task.FromResult(_products.ContainsKey(id));

    public Task<IReadOnlyList<ProductEntity>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<ProductEntity>>(_products.Values.ToList());

    public Task AddAsync(ProductEntity product) => throw new NotSupportedException("No usado por RegisterSaleCommandHandler.");
}

public class FakeSaleRepository : ISaleRepository
{
    private readonly List<SaleEntity> _sales = new();
    private int _nextId = 1;

    public int Count => _sales.Count;

    public Task<SaleEntity?> GetByIdAsync(int id) =>
        Task.FromResult(_sales.FirstOrDefault(s => s.Id == id));

    public Task<List<SaleEntity>> GetAllAsync() => Task.FromResult(_sales.ToList());

    public Task AddAsync(SaleEntity sale)
    {
        // Simula el Id autoincremental que SQL Server asigna al insertar.
        typeof(SaleEntity).GetProperty(nameof(SaleEntity.Id))!.SetValue(sale, _nextId++);
        _sales.Add(sale);
        return Task.CompletedTask;
    }

    public void TruncateTo(int count) => _sales.RemoveRange(count, _sales.Count - count);
}

public class FakeSaleDetailRepository : ISaleDetailRepository
{
    private readonly List<SaleDetailEntity> _details = new();

    public int Count => _details.Count;

    public Task AddAsync(SaleDetailEntity detail)
    {
        _details.Add(detail);
        return Task.CompletedTask;
    }

    public Task<List<SaleDetailEntity>> GetBySaleIdAsync(int saleId) =>
        Task.FromResult(_details.Where(d => d.SaleId == saleId).ToList());

    public void TruncateTo(int count) => _details.RemoveRange(count, _details.Count - count);
}

public class FakeSalesInventoryMovementRepository : IInventoryMovementRepository
{
    private readonly List<InventoryMovementEntity> _movements = new();

    public IReadOnlyList<InventoryMovementEntity> Movements => _movements;

    public Task AddAsync(InventoryMovementEntity movement)
    {
        _movements.Add(movement);
        return Task.CompletedTask;
    }

    public Task<List<InventoryMovementEntity>> GetAllAsync() => Task.FromResult(_movements.ToList());

    public void TruncateTo(int count) => _movements.RemoveRange(count, _movements.Count - count);
}

/// <summary>
/// Simula el rollback REAL de IUnitOfWork.ExecuteInTransactionAsync: toma un
/// snapshot del estado mutable antes de ejecutar la operación y, si lanza,
/// lo restaura antes de relanzar -- igual que haría una transacción real de
/// SQL Server. Sin esto, el test del rollback de punta a punta (venta de dos
/// líneas donde la segunda falla) no probaría nada: los fakes mutan
/// diccionarios/listas compartidos directamente, así que una excepción sin
/// este snapshot/restore dejaría la primera línea decrementada igual.
/// </summary>
public class FakeTransactionalUnitOfWork(
    FakeSalesInventoryRepository inventoryRepository,
    FakeSaleRepository saleRepository,
    FakeSaleDetailRepository saleDetailRepository,
    FakeSalesInventoryMovementRepository movementRepository) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation)
    {
        var quantitiesSnapshot = inventoryRepository.SnapshotQuantities();
        var salesCountBefore = saleRepository.Count;
        var detailsCountBefore = saleDetailRepository.Count;
        var movementsCountBefore = movementRepository.Movements.Count;

        try
        {
            return await operation();
        }
        catch
        {
            inventoryRepository.RestoreQuantities(quantitiesSnapshot);
            saleRepository.TruncateTo(salesCountBefore);
            saleDetailRepository.TruncateTo(detailsCountBefore);
            movementRepository.TruncateTo(movementsCountBefore);
            throw;
        }
    }
}
