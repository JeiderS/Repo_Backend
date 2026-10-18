using Inventory.Domain.ModuleActions.Entity;
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

    public async Task<IReadOnlyList<int>> GetActionIdsAsync(int moduleId)
    {
        return await context.ModuleActions
            .Where(ma => ma.ModuleId == moduleId)
            .Select(ma => ma.ActionId)
            .ToListAsync();
    }

    public async Task<IReadOnlyDictionary<int, IReadOnlyList<int>>> GetActionIdsByModuleAsync()
    {
        var rows = await context.ModuleActions
            .Select(ma => new { ma.ModuleId, ma.ActionId })
            .ToListAsync();

        return rows
            .GroupBy(r => r.ModuleId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<int>)g.Select(r => r.ActionId).ToList());
    }

    public async Task<IReadOnlySet<int>> GetActionIdsInUseByRolesAsync(int moduleId, IEnumerable<int> actionIds)
    {
        var ids = actionIds.ToList();
        var inUse = await context.RoleActions
            .Where(ra => ra.ModuleId == moduleId && ids.Contains(ra.ActionId))
            .Select(ra => ra.ActionId)
            .Distinct()
            .ToListAsync();
        return inUse.ToHashSet();
    }

    public async Task AddModuleActionsAsync(int moduleId, IEnumerable<int> actionIds)
    {
        foreach (var actionId in actionIds.Distinct())
        {
            await context.ModuleActions.AddAsync(ModuleActionEntity.Create(moduleId, actionId));
        }
    }

    public async Task RemoveModuleActionsAsync(int moduleId, IEnumerable<int> actionIds)
    {
        var idSet = actionIds.ToHashSet();
        if (idSet.Count == 0) return;

        var toRemove = await context.ModuleActions
            .Where(ma => ma.ModuleId == moduleId && idSet.Contains(ma.ActionId))
            .ToListAsync();

        context.ModuleActions.RemoveRange(toRemove);
    }
}
