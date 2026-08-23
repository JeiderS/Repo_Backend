using System.Security.Claims;
using Inventory.Application.Common.Interfaces;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Middleware;

/// <summary>
/// Rejects any authenticated request from a deactivated user, even when it
/// carries a still-valid, previously issued JWT (spec: user-administration,
/// "Immediate Deactivation Enforcement"). Runs after
/// UseTenantClaimValidationMiddleware — see design.md "ActiveUserValidationMiddleware
/// runs after TenantClaimValidationMiddleware" decision: tenant identity must be
/// proven first, since this middleware performs the first tenant-scoped DB read
/// in the pipeline, and a cross-tenant user-status oracle must be avoided.
/// </summary>
public class ActiveUserValidationMiddleware(
    RequestDelegate next,
    ILogger<ActiveUserValidationMiddleware> logger)
{
    private const string InactiveUserMessage = "Usuario inactivo. Contacte a soporte.";

    public async Task InvokeAsync(HttpContext context, DataBaseContext dbContext, IActiveUserCache activeUserCache)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                await TenantResponseWriter.WriteFailureAsync(
                    context, StatusCodes.Status403Forbidden, InactiveUserMessage);
                return;
            }

            if (!activeUserCache.TryGet(userId, out var isActive))
            {
                isActive = await dbContext.Users
                    .Where(u => u.Id == userId)
                    .Select(u => (bool?)u.IsActive)
                    .FirstOrDefaultAsync() ?? false;

                activeUserCache.Set(userId, isActive);
            }

            if (!isActive)
            {
                logger.LogWarning("Rejected request from inactive user {UserId}", userId);
                await TenantResponseWriter.WriteFailureAsync(
                    context, StatusCodes.Status403Forbidden, InactiveUserMessage);
                return;
            }
        }

        await next(context);
    }
}

public static class ActiveUserValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseActiveUserValidationMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ActiveUserValidationMiddleware>();
}
