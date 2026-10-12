using Inventory.Domain.RoleActions.Entity;
using Inventory.Domain.Roles.Entity;
using Inventory.Domain.Roles.Repositories;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Repositories;

public class RoleRepository(DataBaseContext context) : IRoleRepository
{
    public async Task<RoleEntity?> GetByIdAsync(int id)
    {
        return await context.Roles.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IReadOnlyList<RoleEntity>> GetAllAsync()
    {
        return await context.Roles
            .OrderBy(r => r.Id)
            .ToListAsync();
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeRoleId = null)
    {
        return await context.Roles
            .AnyAsync(r => r.Name == name && (excludeRoleId == null || r.Id != excludeRoleId));
    }

    public async Task AddAsync(RoleEntity role)
    {
        await context.Roles.AddAsync(role);
    }

    public async Task<bool> IsSystemAdminAsync(int roleId)
    {
        return await context.Roles
            .Where(r => r.Id == roleId)
            .Select(r => (bool?)r.IsSystemAdmin)
            .FirstOrDefaultAsync() ?? false;
    }

    public async Task<IReadOnlyList<int>> GetActionIdsAsync(int roleId)
    {
        return await context.RoleActions
            .Where(ra => ra.RoleId == roleId)
            .Select(ra => ra.ActionId)
            .ToListAsync();
    }

    public async Task<IReadOnlyDictionary<int, IReadOnlyList<int>>> GetActionIdsByRoleAsync()
    {
        var rows = await context.RoleActions
            .Select(ra => new { ra.RoleId, ra.ActionId })
            .ToListAsync();

        return rows
            .GroupBy(r => r.RoleId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<int>)g.Select(r => r.ActionId).ToList());
    }

    public async Task<IReadOnlyList<string>> GetActionCodesAsync(int roleId)
    {
        return await context.RoleActions
            .Where(ra => ra.RoleId == roleId)
            .Select(ra => ra.Action!.Code)
            .ToListAsync();
    }

    public async Task ReplaceActionsAsync(int roleId, IEnumerable<int> actionIds)
    {
        var current = await context.RoleActions
            .Where(ra => ra.RoleId == roleId)
            .ToListAsync();

        context.RoleActions.RemoveRange(current);

        foreach (var actionId in actionIds.Distinct())
        {
            await context.RoleActions.AddAsync(new RoleActionEntity
            {
                RoleId = roleId,
                ActionId = actionId
            });
        }
    }
}
