using Inventory.Api.Tenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Inventory.Api.Middleware;

/// <summary>
/// Cross-validates the JWT <c>tenant</c> claim against the tenant resolved for
/// this request (spec: tenant-scoped-login, JWT Tenant Claim requirement).
/// Runs after UseAuthentication — see design.md "JWT tenant claim is enforced,
/// not informational" decision. Only fires for authenticated requests, so it
/// does not apply to anonymous endpoints like /register.
///
/// A MISSING tenant claim is treated as a mismatch like any other invalid
/// claim (Judgment Day round 1, both judges CONFIRMED WARNING): tokens issued
/// before this deploy carry no tenant claim, so this is a deliberate, bounded
/// (&lt;= Jwt:ExpiresMinutes) one-time forced re-login, not a bug.
///
/// New, unregistered file until Checkpoint B (design.md Migration/Rollout)
/// wires it into the pipeline, atomically with the JWT tenant claim change —
/// see JwtTokenGenerator.cs (Judgment Day round 2, CONFIRMED CRITICAL).
/// </summary>
public class TenantClaimValidationMiddleware(
    RequestDelegate next,
    ILogger<TenantClaimValidationMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("tenant")?.Value;

            if (!string.Equals(tenantClaim, tenantContext.Key, StringComparison.Ordinal))
            {
                logger.LogWarning(
                    "JWT tenant claim '{Claim}' does not match resolved tenant '{Resolved}'",
                    tenantClaim ?? "(none)", tenantContext.Key);
                await TenantResponseWriter.WriteFailureAsync(
                    context, StatusCodes.Status401Unauthorized, "Tenant mismatch.");
                return;
            }
        }

        await next(context);
    }
}

public static class TenantClaimValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantClaimValidationMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<TenantClaimValidationMiddleware>();
}
