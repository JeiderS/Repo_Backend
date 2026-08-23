using System.Security.Claims;
using Inventory.Api.Auth;
using Inventory.Application.Common.Interfaces;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Middleware;

/// <summary>
/// Derives the authenticated user's effective Action-code claims from live
/// RoleActions/Roles state and REPLACES the identity's role-claim set with
/// them (design.md D2) — it does not append to whatever the presented JWT
/// carried. A free-text role name (e.g. a tenant-created role literally named
/// "UsersView" or "SystemAdmin") could otherwise collide with a privileged
/// claim via a pre-deploy JWT still valid for up to Jwt:ExpiresMinutes after
/// this deploy, so every existing role/system_admin claim is stripped first.
/// RoleActions is the single enforced permission source from this checkpoint
/// on; nothing in the presented JWT is trusted for authorization.
/// Runs after UseActiveUserValidationMiddleware and before UseAuthorization
/// (design.md D8) — see Program.cs.
/// </summary>
public class PermissionClaimsMiddleware(RequestDelegate next)
{
    private const int NoRoleSentinel = 0;

    public async Task InvokeAsync(HttpContext context, DataBaseContext dbContext, IPermissionCache permissionCache)
    {
        if (context.User.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
        {
            StripExistingPermissionClaims(identity);

            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is not null && int.TryParse(userIdClaim.Value, out var userId))
            {
                var roleId = await ResolveRoleIdAsync(userId, dbContext, permissionCache);

                if (roleId != NoRoleSentinel)
                {
                    var permissions = await ResolveRolePermissionsAsync(roleId, dbContext, permissionCache);
                    ApplyDerivedClaims(identity, permissions);
                }
            }
        }

        await next(context);
    }

    private static void StripExistingPermissionClaims(ClaimsIdentity identity)
    {
        foreach (var staleClaim in identity.FindAll(identity.RoleClaimType).ToList())
        {
            identity.TryRemoveClaim(staleClaim);
        }

        foreach (var staleClaim in identity.FindAll(PermissionClaimTypes.SystemAdmin).ToList())
        {
            identity.TryRemoveClaim(staleClaim);
        }
    }

    private static async Task<int> ResolveRoleIdAsync(
        int userId, DataBaseContext dbContext, IPermissionCache permissionCache)
    {
        if (permissionCache.TryGetRoleId(userId, out var cachedRoleId))
            return cachedRoleId;

        var roleId = await dbContext.Users
            .Where(u => u.Id == userId && u.RoleId != null)
            .Select(u => (int?)u.RoleId!.Value)
            .FirstOrDefaultAsync() ?? NoRoleSentinel;

        permissionCache.SetRoleId(userId, roleId);

        return roleId;
    }

    private static async Task<RolePermissions> ResolveRolePermissionsAsync(
        int roleId, DataBaseContext dbContext, IPermissionCache permissionCache)
    {
        if (permissionCache.TryGetRolePermissions(roleId, out var cachedPermissions))
            return cachedPermissions;

        var isSystemAdmin = await dbContext.Roles
            .Where(r => r.Id == roleId)
            .Select(r => (bool?)r.IsSystemAdmin)
            .FirstOrDefaultAsync() ?? false;

        var actionCodes = await dbContext.RoleActions
            .Where(ra => ra.RoleId == roleId)
            .Select(ra => ra.Action!.Code)
            .ToListAsync();

        var permissions = new RolePermissions(roleId, isSystemAdmin, actionCodes);
        permissionCache.SetRolePermissions(permissions);

        return permissions;
    }

    private static void ApplyDerivedClaims(ClaimsIdentity identity, RolePermissions permissions)
    {
        foreach (var actionCode in permissions.ActionCodes)
        {
            identity.AddClaim(new Claim(identity.RoleClaimType, actionCode));
        }

        if (permissions.IsSystemAdmin)
        {
            identity.AddClaim(new Claim(PermissionClaimTypes.SystemAdmin, "true"));
        }
    }
}

public static class PermissionClaimsMiddlewareExtensions
{
    public static IApplicationBuilder UsePermissionClaimsMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<PermissionClaimsMiddleware>();
}
