using Inventory.Domain.Roles.DomainRoles;
using Inventory.Domain.RoleActions.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Roles.DomainService.Impl;

public class RoleActionAssignService(DataBaseContext context) : IRoleActionAssignService
{
    public async Task<IReadOnlyList<int>> GetActionIdsAsync(int roleId)
    {
        return await context.RoleActions
            .Where(ra => ra.RoleId == roleId)
            .Select(ra => ra.ActionId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<int>> GetExistingActionIdsAsync(IEnumerable<int> actionIds)
    {
        var ids = actionIds.ToList();
        return await context.Actions
            .Where(a => ids.Contains(a.Id))
            .Select(a => a.Id)
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
