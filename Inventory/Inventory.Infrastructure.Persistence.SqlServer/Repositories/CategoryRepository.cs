using Inventory.Domain.Categories.Entity;
using Inventory.Domain.Categories.Repositories;
using Inventory.Infrastructure.Persistence.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.SqlServer.Repositories;

public class CategoryRepository(DataBaseContext context) : ICategoryRepository
{
    public async Task<CategoryEntity?> GetByIdAsync(int id)
    {
        return await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Categories.AnyAsync(c => c.Id == id);
    }

    public async Task<IReadOnlyList<CategoryEntity>> GetAllAsync()
    {
        return await context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeCategoryId = null)
    {
        return await context.Categories
            .AnyAsync(c => c.Name == name && (excludeCategoryId == null || c.Id != excludeCategoryId));
    }

    public async Task AddAsync(CategoryEntity category)
    {
        await context.Categories.AddAsync(category);
    }
}
