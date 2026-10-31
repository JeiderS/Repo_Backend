using Inventory.Domain.InventoryMovements.Entity;
using Inventory.Domain.InventoryMovements.Repositories;
using Inventory.Infrastructure.Persistence.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.SqlServer.Repositories;

public class InventoryMovementRepository(DataBaseContext context) : IInventoryMovementRepository
{
    public async Task AddAsync(InventoryMovementEntity movement)
    {
        await context.InventoryMovements.AddAsync(movement);
    }

    public async Task<List<InventoryMovementEntity>> GetAllAsync()
    {
        return await context.InventoryMovements.ToListAsync();
    }
}
