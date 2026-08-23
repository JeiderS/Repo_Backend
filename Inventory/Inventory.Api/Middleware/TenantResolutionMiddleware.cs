using Inventory.Api.Tenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Inventory.Api.Middleware;

/// <summary>
/// Resolves the tenant from the request Host header before any authentication
/// or business logic executes (spec: tenant-resolution). Placed after
/// UseCors and before UseAuthentication — see design.md "middleware placement
/// and fail-fast response" decision. New, unregistered file until Checkpoint B
/// (design.md Migration/Rollout) wires it into the pipeline.
/// </summary>
public class TenantResolutionMiddleware(
    RequestDelegate next,
    IOptions<TenantRegistryOptions> options,
    ILogger<TenantResolutionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
    {
        var host = context.Request.Host.Host;
        var resolution = TenantResolver.ResolveKey(host, options.Value);

        switch (resolution)
        {
            case TenantResolution.Resolved resolved:
                var entry = options.Value.Registry[resolved.Key];
                tenantContext.Set(resolved.Key, entry.Name, entry.ConnectionString);
                await next(context);
                break;

            case TenantResolution.UnknownTenant unknown:
                // Host is logged for operability, never echoed into the response
                // body — the enumeration-oracle risk in design.md Threat Matrix
                // is accepted, but we still avoid amplifying it further.
                logger.LogWarning(
                    "Unknown tenant for host {Host} (resolved key: {Key})", host, unknown.Key);
                await TenantResponseWriter.WriteFailureAsync(
                    context, StatusCodes.Status404NotFound, "Unknown tenant.");
                break;

            case TenantResolution.UnsupportedHost:
                logger.LogWarning("Host {Host} matches no configured root domain", host);
                await TenantResponseWriter.WriteFailureAsync(
                    context, StatusCodes.Status400BadRequest, "Unsupported host.");
                break;
        }
    }
}

public static class TenantResolutionMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantResolutionMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<TenantResolutionMiddleware>();
}
