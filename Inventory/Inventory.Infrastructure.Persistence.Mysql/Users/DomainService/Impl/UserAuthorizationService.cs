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
            .Where(u => u.Id == userId && u.RoleId != null)
            .Select(u => u.Role!.Name)
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

        // RoleActions es la fuente única desde Checkpoint B (design.md D5/D9);
        // RoleModules ya no se lee aquí. ModulePermission se reconstruye a
        // partir del sufijo de verbo de Action.Code (Module.Name + verbo, ver
        // 03_Actions_Seed.sql), no de columnas Can* por fila.
        var roleActions = await context.RoleActions
            .Where(ra => roleIds.Contains(ra.RoleId))
            .Include(ra => ra.Action)
                .ThenInclude(a => a!.Module)
            .ToListAsync();

        var permissions = roleActions
            .GroupBy(ra => ra.Action!.Module!.Name)
            .Select(g => new ModulePermission(
                Module: g.Key,
                CanView: g.Any(x => x.Action!.Code.EndsWith(PermissionAction.View, StringComparison.Ordinal)),
                CanCreate: g.Any(x => x.Action!.Code.EndsWith(PermissionAction.Create, StringComparison.Ordinal)),
                CanEdit: g.Any(x => x.Action!.Code.EndsWith(PermissionAction.Edit, StringComparison.Ordinal)),
                CanDelete: g.Any(x => x.Action!.Code.EndsWith(PermissionAction.Delete, StringComparison.Ordinal))))
            .ToList();

        return new UserAuthorizationData(roleNames, permissions);
    }

    private async Task<List<int>> GetRoleIdsAsync(int userId)
    {
        return await context.Users
            .Where(u => u.Id == userId && u.RoleId != null)
            .Select(u => u.RoleId!.Value)
            .ToListAsync();
    }
}
