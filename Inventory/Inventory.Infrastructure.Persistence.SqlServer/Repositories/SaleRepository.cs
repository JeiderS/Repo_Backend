using Inventory.Domain.Sales.Entity;
using Inventory.Domain.Sales.Repositories;
using Inventory.Infrastructure.Persistence.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.SqlServer.Repositories;

public class SaleRepository(DataBaseContext context) : ISaleRepository
{
    public async Task<SaleEntity?> GetByIdAsync(int id)
    {
        return await context.Sales.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<SaleEntity>> GetAllAsync()
    {
        return await context.Sales.ToListAsync();
    }

    public async Task AddAsync(SaleEntity sale)
    {
        await context.Sales.AddAsync(sale);
    }
}
