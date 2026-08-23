using Inventory.Application.Common.Interfaces;

namespace Inventory.Api.Tenancy;

/// <summary>
/// Scoped, mutable holder for the ambient tenant of the current request.
/// Populated once by <see cref="Middleware.TenantResolutionMiddleware"/> before
/// any downstream code (DbContext construction, JWT issuance) reads it. Not yet
/// registered in the DI container — see Checkpoint A / Checkpoint B in
/// design.md Migration/Rollout.
/// </summary>
public class TenantContext : ITenantContext
{
    public string Key { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string ConnectionString { get; private set; } = string.Empty;

    public void Set(string key, string name, string connectionString)
    {
        Key = key;
        Name = name;
        ConnectionString = connectionString;
    }
}
