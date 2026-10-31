using Inventory.Domain.SaleDetails.Entity;
using Inventory.Domain.SaleDetails.Repositories;
using Inventory.Infrastructure.Persistence.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.SqlServer.Repositories;

public class SaleDetailRepository(DataBaseContext context) : ISaleDetailRepository
{
    public async Task AddAsync(SaleDetailEntity detail)
    {
        await context.SaleDetails.AddAsync(detail);
    }

    public async Task<List<SaleDetailEntity>> GetBySaleIdAsync(int saleId)
    {
        return await context.SaleDetails.Where(d => d.SaleId == saleId).ToListAsync();
    }
}
