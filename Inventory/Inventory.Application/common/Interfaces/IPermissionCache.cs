namespace Inventory.Application.Common.Interfaces;

/// <summary>
/// The Action codes and IsSystemAdmin flag effective for a given role, resolved
/// live from RoleActions/Roles state.
/// </summary>
public sealed record RolePermissions(int RoleId, bool IsSystemAdmin, IReadOnlyList<string> ActionCodes);

/// <summary>
/// Two-level, tenant-scoped cache backing PermissionClaimsMiddleware:
/// userId -> roleId ("perm-user:{tenantKey}:{userId}", 0 = no role — RoleId is
/// IDENTITY, never 0, same sentinel ModuleMenuService.RootKey already uses),
/// and roleId -> RolePermissions ("perm-role:{tenantKey}:{roleId}"). Locked on
/// roleId for the permission set because RoleActions mutation is the real
/// invalidation trigger, and a userId-keyed set would need fan-out
/// invalidation IMemoryCache cannot enumerate (design.md D3).
/// </summary>
public interface IPermissionCache
{
    bool TryGetRoleId(int userId, out int roleId);

    /// <summary>60s absolute expiration — see design.md D3 "TTL" decision (sliding rejected).</summary>
    void SetRoleId(int userId, int roleId);

    void InvalidateUser(int userId);

    bool TryGetRolePermissions(int roleId, out RolePermissions permissions);

    /// <summary>60s absolute expiration — see design.md D3 "TTL" decision (sliding rejected).</summary>
    void SetRolePermissions(RolePermissions permissions);

    void InvalidateRole(int roleId);
}
