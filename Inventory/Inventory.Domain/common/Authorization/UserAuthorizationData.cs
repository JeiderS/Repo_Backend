namespace Inventory.Domain.Common.Authorization;

public record UserAuthorizationData(
    IReadOnlyList<string> Roles,
    IReadOnlyList<ModulePermission> Permissions)
{
    public static UserAuthorizationData Empty { get; } =
        new(Array.Empty<string>(), Array.Empty<ModulePermission>());
}
