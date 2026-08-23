namespace Inventory.Application.Common.Interfaces;

/// <summary>
/// Per-user "is active" boolean cache, tenant-scoped, backing
/// ActiveUserValidationMiddleware. Key format: "active-user:{tenantKey}:{userId}"
/// — user ids are per-tenant IDENTITY values and collide across tenant DBs, so
/// the tenant key must always be part of the key (design.md decision).
/// </summary>
public interface IActiveUserCache
{
    bool TryGet(int userId, out bool isActive);

    /// <summary>60s absolute expiration — see design.md "TTL" decision (sliding rejected).</summary>
    void Set(int userId, bool isActive);

    void Invalidate(int userId);
}
