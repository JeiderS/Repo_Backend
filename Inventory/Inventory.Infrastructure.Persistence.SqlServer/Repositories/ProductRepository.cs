using Inventory.Domain.Products.Entity;
using Inventory.Domain.Products.Repositories;
using Inventory.Infrastructure.Persistence.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.SqlServer.Repositories;

public class ProductRepository(DataBaseContext context) : IProductRepository
{
    public async Task<ProductEntity?> GetByIdAsync(int id)
    {
        return await context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Products.AnyAsync(p => p.Id == id);
    }

    public async Task<IReadOnlyList<ProductEntity>> GetAllAsync()
    {
        return await context.Products
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task AddAsync(ProductEntity product)
    {
        await context.Products.AddAsync(product);
    }
}
