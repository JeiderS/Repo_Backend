using Inventory.Domain.Inventories.Entity;
using Inventory.Domain.Inventories.Repositories;
using Inventory.Infrastructure.Persistence.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.SqlServer.Repositories;

public class InventoryRepository(DataBaseContext context) : IInventoryRepository
{
    public async Task<InventoryEntity?> GetByIdAsync(int id)
    {
        return await context.Inventories.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<InventoryEntity>> GetAllAsync()
    {
        return await context.Inventories.ToListAsync();
    }

    public async Task AddAsync(InventoryEntity inventory)
    {
        await context.Inventories.AddAsync(inventory);
    }

    public async Task<decimal?> TryIncreaseQuantityAsync(int inventoryId, decimal amount)
    {
        var rows = await context.Inventories
            .Where(i => i.Id == inventoryId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(i => i.Quantity, i => i.Quantity + amount)
                .SetProperty(i => i.UpdatedAt, _ => DateTime.UtcNow));

        return rows > 0 ? await ReadCommittedQuantityAsync(inventoryId) : null;
    }

    public async Task<decimal?> TryDecreaseQuantityAsync(int inventoryId, decimal amount)
    {
        var rows = await context.Inventories
            .Where(i => i.Id == inventoryId && i.Quantity >= amount)
            .ExecuteUpdateAsync(s => s
                .SetProperty(i => i.Quantity, i => i.Quantity - amount)
                .SetProperty(i => i.UpdatedAt, _ => DateTime.UtcNow));

        return rows > 0 ? await ReadCommittedQuantityAsync(inventoryId) : null;
    }

    // ExecuteUpdateAsync bypasses the change tracker, so an InventoryEntity a
    // caller already loaded via GetByIdAsync never reflects this update. This
    // untracked re-read is the only way to hand back the true post-update
    // value instead of a stale one computed from the old in-memory entity.
    private async Task<decimal> ReadCommittedQuantityAsync(int inventoryId) =>
        await context.Inventories.AsNoTracking()
            .Where(i => i.Id == inventoryId)
            .Select(i => i.Quantity)
            .FirstAsync();
}
