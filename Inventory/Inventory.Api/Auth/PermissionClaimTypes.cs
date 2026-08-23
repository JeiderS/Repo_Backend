namespace Inventory.Api.Auth;

/// <summary>
/// Distinct claim type for the system-admin flag, deliberately not
/// ClaimTypes.Role (design.md D1). Role names are free text
/// (CreateRoleCommand.Name, uniqueness-checked only): a tenant admin could
/// create a role literally named "SystemAdmin" today, and a user in it who
/// logs in before this deploy would hold ClaimTypes.Role = "SystemAdmin" in a
/// token still valid for up to Jwt:ExpiresMinutes after deploy. A dedicated
/// claim type closes that window structurally — it cannot be satisfied by any
/// Action code or tenant-chosen role name.
/// </summary>
public static class PermissionClaimTypes
{
    public const string SystemAdmin = "system_admin";
}
