using System.Security.Claims;
using Inventory.Domain.Users.DomainUsers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Inventory.Api.Auth;

/// <summary>
/// Autoriza una acción verificando en BD (RoleModules) si el usuario autenticado
/// tiene el permiso "Modulo.Accion" (ver <see cref="Inventory.Domain.Common.Authorization.PermissionAction"/>).
/// Se consulta en cada request a propósito: el JWT no lleva permisos, así un
/// cambio de permisos en BD aplica de inmediato sin esperar a que el token expire.
/// Reutilizable por cualquier controlador: [HasPermission("Drivers", PermissionAction.View)].
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class HasPermissionAttribute(string module, string action) : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user.Identity is null || !user.Identity.IsAuthenticated)
        {
            context.Result = new ChallengeResult();
            return;
        }

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            context.Result = new ForbidResult();
            return;
        }

        var authorizationService = context.HttpContext.RequestServices
            .GetRequiredService<IUserAuthorizationService>();

        var hasPermission = await authorizationService.HasPermissionAsync(userId, module, action);

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }
}
