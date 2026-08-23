using Inventory.Api.Auth;
using Inventory.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
using Xunit;

namespace Inventory.Tests.Auth;

/// <summary>
/// Task 3.1 (Phase 3, Checkpoint B tests) — unit coverage for
/// <see cref="ActiveUserCache"/>: key composition includes the tenant key
/// (spec: user-administration, "Immediate Deactivation Enforcement" — the
/// design.md decision calls out that user ids are per-tenant IDENTITY values
/// and collide across tenant DBs, so omitting the tenant key would be a
/// cross-tenant correctness bug) and the TTL is absolute, not sliding
/// (design.md: "Sliding expiration rejected — under steady traffic it could
/// keep a stale true alive indefinitely").
///
/// Uses a real <see cref="MemoryCache"/> with a manually-advanced
/// <see cref="TimeProvider"/> so expiration can be asserted deterministically
/// without waiting on a real clock.
/// </summary>
public class ActiveUserCacheTests
{
    private sealed class FakeTenantContext(string key) : ITenantContext
    {
        public string Key { get; } = key;
        public string Name => $"Tenant {Key}";
        public string ConnectionString => "Server=fake;";
    }

#pragma warning disable CS0618 // ISystemClock is obsolete in favor of TimeProvider, but it's what
                               // this resolved package version (8.0.1) still exposes via MemoryCacheOptions.Clock.
    private sealed class ManualClock(DateTimeOffset start) : ISystemClock
    {
        public DateTimeOffset UtcNow { get; private set; } = start;

        public void Advance(TimeSpan delta) => UtcNow += delta;
    }
#pragma warning restore CS0618

    [Fact]
    public void Set_KeyIncludesTenantKey_EntryIsNotVisibleFromADifferentTenant()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var acmeCache = new ActiveUserCache(memoryCache, new FakeTenantContext("acme"));
        var globexCache = new ActiveUserCache(memoryCache, new FakeTenantContext("globex"));

        acmeCache.Set(userId: 5, isActive: true);

        // Same underlying IMemoryCache, same numeric userId, different tenant:
        // if the key omitted the tenant, this would be a HIT (cross-tenant bug).
        var globexHit = globexCache.TryGet(userId: 5, out var globexValue);
        var acmeHit = acmeCache.TryGet(userId: 5, out var acmeValue);

        Assert.False(globexHit);
        Assert.False(globexValue);
        Assert.True(acmeHit);
        Assert.True(acmeValue);
    }

    [Fact]
    public void Set_UsesAbsoluteExpiration_RepeatedAccessDoesNotExtendTtl()
    {
        var start = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var clock = new ManualClock(start);
#pragma warning disable CS0618
        using var memoryCache = new MemoryCache(new MemoryCacheOptions { Clock = clock });
#pragma warning restore CS0618
        var cache = new ActiveUserCache(memoryCache, new FakeTenantContext("acme"));

        cache.Set(userId: 7, isActive: true);

        // Access repeatedly before the 60s absolute TTL elapses. If expiration
        // were SLIDING, each TryGet below would push the expiry further out.
        clock.Advance(TimeSpan.FromSeconds(59));
        var stillCachedBeforeExpiry = cache.TryGet(userId: 7, out _);

        clock.Advance(TimeSpan.FromSeconds(30)); // now 89s since Set — past the fixed 60s absolute mark
        var expiredAfterAbsoluteWindow = cache.TryGet(userId: 7, out var expiredValue);

        Assert.True(stillCachedBeforeExpiry);
        Assert.False(expiredAfterAbsoluteWindow);
        Assert.False(expiredValue);
    }
}
