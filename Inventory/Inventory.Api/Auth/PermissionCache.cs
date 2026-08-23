using Inventory.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Inventory.Api.Auth;

/// <summary>
/// IMemoryCache-backed IPermissionCache, mirroring ActiveUserCache's shape
/// (design.md D9 file/registration conventions). Two independent entries per
/// tenant-scoped key: "perm-user:{tenantKey}:{userId}" (roleId, 0 = no role)
/// and "perm-role:{tenantKey}:{roleId}" (RolePermissions). Registered
/// AddScoped because it composes the singleton IMemoryCache with the scoped
/// ITenantContext.
/// </summary>
public class PermissionCache(IMemoryCache cache, ITenantContext tenantContext) : IPermissionCache
{
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);

    public bool TryGetRoleId(int userId, out int roleId)
    {
        return cache.TryGetValue(BuildUserKey(userId), out roleId);
    }

    public void SetRoleId(int userId, int roleId)
    {
        cache.Set(BuildUserKey(userId), roleId, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = Ttl
        });
    }

    public void InvalidateUser(int userId)
    {
        cache.Remove(BuildUserKey(userId));
    }

    public bool TryGetRolePermissions(int roleId, out RolePermissions permissions)
    {
        return cache.TryGetValue(BuildRoleKey(roleId), out permissions!);
    }

    public void SetRolePermissions(RolePermissions permissions)
    {
        cache.Set(BuildRoleKey(permissions.RoleId), permissions, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = Ttl
        });
    }

    public void InvalidateRole(int roleId)
    {
        cache.Remove(BuildRoleKey(roleId));
    }

    private string BuildUserKey(int userId) => $"perm-user:{tenantContext.Key}:{userId}";

    private string BuildRoleKey(int roleId) => $"perm-role:{tenantContext.Key}:{roleId}";
}
