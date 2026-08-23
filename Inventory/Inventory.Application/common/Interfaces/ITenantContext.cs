namespace Inventory.Application.Common.Interfaces;

/// <summary>
/// Request-scoped ambient tenant, resolved once per request by
/// TenantResolutionMiddleware and populated before any Application-layer code
/// runs. Sits beside <see cref="IPasswordHasher"/> / <see cref="IJwtTokenGenerator"/>
/// since Api references Application.
/// </summary>
public interface ITenantContext
{
    string Key { get; }

    string Name { get; }

    string ConnectionString { get; }
}
