using Inventory.Api.Auth;
using Inventory.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
using Xunit;

namespace Inventory.Tests.Auth;

/// <summary>
/// Task 4.1 (Phase 4, Checkpoint B tests) — unit coverage for
/// <see cref="PermissionCache"/>: key composition includes the tenant key
/// (same cross-tenant-collision argument as <see cref="ActiveUserCacheTests"/>,
/// since userId/roleId are per-tenant IDENTITY values), TTL is absolute not
/// sliding (design.md D3 "60s absolute TTL"), and roleId = 0 (the "no role"
/// sentinel <c>PermissionClaimsMiddleware.NoRoleSentinel</c>) round-trips as
/// a genuine cache HIT with value 0 — distinguishable from a cache MISS
/// (<see cref="IPermissionCache.TryGetRoleId"/> returning false) — which is
/// what lets the middleware skip the RolePermissions lookup entirely for a
/// user with no role, on every request after the first, without a DB read.
///
/// Uses a real <see cref="MemoryCache"/> with a manually-advanced
/// <see cref="TimeProvider"/>-equivalent clock, same pattern as
/// <see cref="ActiveUserCacheTests"/>, so expiration is asserted
/// deterministically without a real clock wait.
/// </summary>
public class PermissionCacheTests
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
    public void SetRoleId_KeyIncludesTenantKey_EntryIsNotVisibleFromADifferentTenant()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var acmeCache = new PermissionCache(memoryCache, new FakeTenantContext("acme"));
        var globexCache = new PermissionCache(memoryCache, new FakeTenantContext("globex"));

        acmeCache.SetRoleId(userId: 5, roleId: 3);

        // Same underlying IMemoryCache, same numeric userId, different
        // tenant: if the key omitted the tenant, this would be a HIT.
        var globexHit = globexCache.TryGetRoleId(userId: 5, out var globexRoleId);
        var acmeHit = acmeCache.TryGetRoleId(userId: 5, out var acmeRoleId);

        Assert.False(globexHit);
        Assert.Equal(0, globexRoleId);
        Assert.True(acmeHit);
        Assert.Equal(3, acmeRoleId);
    }

    [Fact]
    public void SetRolePermissions_KeyIncludesTenantKey_EntryIsNotVisibleFromADifferentTenant()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var acmeCache = new PermissionCache(memoryCache, new FakeTenantContext("acme"));
        var globexCache = new PermissionCache(memoryCache, new FakeTenantContext("globex"));

        var permissions = new RolePermissions(RoleId: 9, IsSystemAdmin: true, ActionCodes: new[] { "UsersView" });
        acmeCache.SetRolePermissions(permissions);

        var globexHit = globexCache.TryGetRolePermissions(roleId: 9, out _);
        var acmeHit = acmeCache.TryGetRolePermissions(roleId: 9, out var acmePermissions);

        Assert.False(globexHit);
        Assert.True(acmeHit);
        Assert.Equal(permissions, acmePermissions);
    }

    [Fact]
    public void SetRoleId_UsesAbsoluteExpiration_RepeatedAccessDoesNotExtendTtl()
    {
        var start = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var clock = new ManualClock(start);
#pragma warning disable CS0618
        using var memoryCache = new MemoryCache(new MemoryCacheOptions { Clock = clock });
#pragma warning restore CS0618
        var cache = new PermissionCache(memoryCache, new FakeTenantContext("acme"));

        cache.SetRoleId(userId: 7, roleId: 4);

        // Access repeatedly before the 60s absolute TTL elapses. If
        // expiration were SLIDING, each TryGetRoleId below would push the
        // expiry further out.
        clock.Advance(TimeSpan.FromSeconds(59));
        var stillCachedBeforeExpiry = cache.TryGetRoleId(userId: 7, out _);

        clock.Advance(TimeSpan.FromSeconds(30)); // 89s since Set — past the fixed 60s absolute mark
        var expiredAfterAbsoluteWindow = cache.TryGetRoleId(userId: 7, out var expiredRoleId);

        Assert.True(stillCachedBeforeExpiry);
        Assert.False(expiredAfterAbsoluteWindow);
        Assert.Equal(0, expiredRoleId);
    }

    [Fact]
    public void SetRolePermissions_UsesAbsoluteExpiration_RepeatedAccessDoesNotExtendTtl()
    {
        var start = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var clock = new ManualClock(start);
#pragma warning disable CS0618
        using var memoryCache = new MemoryCache(new MemoryCacheOptions { Clock = clock });
#pragma warning restore CS0618
        var cache = new PermissionCache(memoryCache, new FakeTenantContext("acme"));
        var permissions = new RolePermissions(RoleId: 4, IsSystemAdmin: false, ActionCodes: new[] { "UsersView" });

        cache.SetRolePermissions(permissions);

        clock.Advance(TimeSpan.FromSeconds(59));
        var stillCachedBeforeExpiry = cache.TryGetRolePermissions(roleId: 4, out _);

        clock.Advance(TimeSpan.FromSeconds(30));
        var expiredAfterAbsoluteWindow = cache.TryGetRolePermissions(roleId: 4, out _);

        Assert.True(stillCachedBeforeExpiry);
        Assert.False(expiredAfterAbsoluteWindow);
    }

    [Fact]
    public void SetRoleId_WithNoRoleSentinelZero_RoundTripsAsACacheHitNotAMiss()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = new PermissionCache(memoryCache, new FakeTenantContext("acme"));

        // Never cached: a genuine miss.
        var missBeforeSet = cache.TryGetRoleId(userId: 11, out var missValue);

        // PermissionClaimsMiddleware caches the "no role" sentinel (0)
        // exactly like any other roleId, so a user with no role does not
        // force a DB read on every subsequent request either.
        cache.SetRoleId(userId: 11, roleId: 0);
        var hitAfterSet = cache.TryGetRoleId(userId: 11, out var hitValue);

        Assert.False(missBeforeSet);
        Assert.Equal(0, missValue);
        Assert.True(hitAfterSet);
        Assert.Equal(0, hitValue);
    }
}
