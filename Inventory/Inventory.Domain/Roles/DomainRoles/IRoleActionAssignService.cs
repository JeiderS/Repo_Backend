namespace Inventory.Domain.Roles.DomainRoles;

public interface IRoleActionAssignService
{
    Task<IReadOnlyList<int>> GetActionIdsAsync(int roleId);

    /// <summary>
    /// Filters the given ids down to the ones that actually exist in the
    /// seeded Actions catalog, so a handler can reject an unknown id
    /// (spec: role-permission-management, "Assigning a non-existent action is rejected").
    /// </summary>
    Task<IReadOnlyList<int>> GetExistingActionIdsAsync(IEnumerable<int> actionIds);

    /// <summary>
    /// Full replace: removes every RoleAction currently linked to the role and
    /// inserts one row per given actionId.
    /// </summary>
    Task ReplaceActionsAsync(int roleId, IEnumerable<int> actionIds);
}
