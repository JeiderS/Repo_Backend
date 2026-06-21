using Inventory.Domain.Common.Authorization;
using Inventory.Domain.Users.DomainUsers;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Users.DomainService.Impl;

public class UserAuthorizationService(DataBaseContext context) : IUserAuthorizationService
{
    public async Task<IReadOnlyList<string>> GetRolesAsync(int userId)
    {
        return await context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Roles.Select(r => r.Name))
            .ToListAsync();
    }

    public async Task<UserAuthorizationData> GetAuthorizationDataAsync(int userId)
    {
        var roleIds = await GetRoleIdsAsync(userId);

        if (roleIds.Count == 0)
            return UserAuthorizationData.Empty;

        var roleNames = await context.Roles
            .Where(r => roleIds.Contains(r.Id))
            .Select(r => r.Name)
            .ToListAsync();

        var roleModules = await context.RoleModules
            .Where(rm => roleIds.Contains(rm.RoleId))
            .Include(rm => rm.Module)
            .ToListAsync();

        var permissions = roleModules
            .GroupBy(rm => rm.Module!.Name)
            .Select(g => new ModulePermission(
                Module: g.Key,
                CanView: g.Any(x => x.CanView),
                CanCreate: g.Any(x => x.CanCreate),
                CanEdit: g.Any(x => x.CanEdit),
                CanDelete: g.Any(x => x.CanDelete)))
            .ToList();

        return new UserAuthorizationData(roleNames, permissions);
    }

    public async Task<bool> HasPermissionAsync(int userId, string module, string action)
    {
        var roleIds = await GetRoleIdsAsync(userId);
        if (roleIds.Count == 0)
            return false;

        var roleModules = await context.RoleModules
            .Where(rm => roleIds.Contains(rm.RoleId))
            .Include(rm => rm.Module)
            .Where(rm => rm.Module!.Name == module)
            .ToListAsync();

        return action switch
        {
            PermissionAction.View => roleModules.Any(rm => rm.CanView),
            PermissionAction.Create => roleModules.Any(rm => rm.CanCreate),
            PermissionAction.Edit => roleModules.Any(rm => rm.CanEdit),
            PermissionAction.Delete => roleModules.Any(rm => rm.CanDelete),
            _ => false
        };
    }

    private async Task<List<int>> GetRoleIdsAsync(int userId)
    {
        return await context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Roles.Select(r => r.Id))
            .ToListAsync();
    }
}
