using Inventory.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Inventory.Api.Auth;

/// <summary>
/// IMemoryCache-backed IActiveUserCache — first use of IMemoryCache in this
/// codebase (design.md decision: single-process API, single boolean per user,
/// a distributed cache is unjustified complexity for Phase 1).
/// Registered AddScoped because it composes the singleton IMemoryCache with
/// the scoped ITenantContext.
/// </summary>
public class ActiveUserCache(IMemoryCache cache, ITenantContext tenantContext) : IActiveUserCache
{
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);

    public bool TryGet(int userId, out bool isActive)
    {
        return cache.TryGetValue(BuildKey(userId), out isActive);
    }

    public void Set(int userId, bool isActive)
    {
        cache.Set(BuildKey(userId), isActive, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = Ttl
        });
    }

    public void Invalidate(int userId)
    {
        cache.Remove(BuildKey(userId));
    }

    private string BuildKey(int userId) => $"active-user:{tenantContext.Key}:{userId}";
}
