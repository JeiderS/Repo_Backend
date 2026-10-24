using System.Security.Claims;
using Inventory.Api.Auth;
using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Modules.Repositories;
using Inventory.Domain.Roles.Repositories;
using Inventory.Domain.Users.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

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

    /// <summary>
    /// El cliente declara desde qué módulo necesita el dato (ej. la pantalla
    /// de Roles pidiendo el catálogo de Actions para armar su picker de
    /// permisos). Cuando está presente y resuelve a un módulo activo, los
    /// claims de este request se acotan a SOLO ese módulo — no es un OR fijo
    /// de permisos alternativos, el permiso exigido depende de ESE módulo
    /// puntual. Si el header no está (la mayoría de los endpoints no lo
    /// necesitan) se sigue usando el set completo del rol, sin acotar.
    /// </summary>
    public const string ModuleContextHeaderName = "X-Module-Code";

    public async Task InvokeAsync(
        HttpContext context,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IModuleRepository moduleRepository,
        IPermissionCache permissionCache)
    {
        if (context.User.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
        {
            StripExistingPermissionClaims(identity);

            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is not null && int.TryParse(userIdClaim.Value, out var userId))
            {
                var roleId = await ResolveRoleIdAsync(userId, userRepository, permissionCache);

                if (roleId != NoRoleSentinel)
                {
                    var permissions = await ResolveRolePermissionsAsync(roleId, roleRepository, permissionCache);

                    var moduleScopedCodes = await TryResolveModuleScopedCodesAsync(
                        context, roleId, roleRepository, moduleRepository);

                    ApplyDerivedClaims(
                        identity,
                        moduleScopedCodes is null ? permissions : permissions with { ActionCodes = moduleScopedCodes });
                }
            }
        }

        await next(context);
    }

    private static async Task<IReadOnlyList<string>?> TryResolveModuleScopedCodesAsync(
        HttpContext context, int roleId, IRoleRepository roleRepository, IModuleRepository moduleRepository)
    {
        if (!context.Request.Headers.TryGetValue(ModuleContextHeaderName, out var headerValues)
            || headerValues.Count != 1
            || string.IsNullOrWhiteSpace(headerValues[0]))
        {
            // Header ausente, repetido o vacío: no hay contexto que resolver,
            // se usa el set completo del rol tal como hoy.
            return null;
        }

        var module = await moduleRepository.GetByRouteAsync(headerValues[0]!.Trim());

        if (module is null || !module.IsActive)
        {
            return null;
        }

        return await roleRepository.GetActionCodesByModuleAsync(roleId, module.Id);
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
        int userId, IUserRepository userRepository, IPermissionCache permissionCache)
    {
        if (permissionCache.TryGetRoleId(userId, out var cachedRoleId))
            return cachedRoleId;

        var roleId = await userRepository.GetRoleIdAsync(userId) ?? NoRoleSentinel;

        permissionCache.SetRoleId(userId, roleId);

        return roleId;
    }

    private static async Task<RolePermissions> ResolveRolePermissionsAsync(
        int roleId, IRoleRepository roleRepository, IPermissionCache permissionCache)
    {
        if (permissionCache.TryGetRolePermissions(roleId, out var cachedPermissions))
            return cachedPermissions;

        var isSystemAdmin = await roleRepository.IsSystemAdminAsync(roleId);
        var actionCodes = await roleRepository.GetActionCodesAsync(roleId);

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
