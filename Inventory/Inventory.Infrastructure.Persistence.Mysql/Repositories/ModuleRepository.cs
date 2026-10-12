using Inventory.Domain.Modules.Entity;
using Inventory.Domain.Modules.Repositories;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Repositories;

public class ModuleRepository(DataBaseContext context) : IModuleRepository
{
    public async Task<ModuleEntity?> GetByIdAsync(int id)
    {
        return await context.Modules.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Modules.AnyAsync(m => m.Id == id);
    }

    public async Task<IReadOnlyList<ModuleEntity>> GetAllAsync()
    {
        return await context.Modules
            .OrderBy(m => m.SortOrder)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ModuleEntity>> GetActiveAsync()
    {
        return await context.Modules
            .Where(m => m.IsActive)
            .OrderBy(m => m.SortOrder)
            .ToListAsync();
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeModuleId = null)
    {
        return await context.Modules
            .AnyAsync(m => m.Name == name && (excludeModuleId == null || m.Id != excludeModuleId));
    }

    public async Task AddAsync(ModuleEntity module)
    {
        await context.Modules.AddAsync(module);
    }
}
