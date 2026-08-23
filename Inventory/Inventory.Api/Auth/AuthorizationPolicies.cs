namespace Inventory.Api.Auth;

/// <summary>
/// Named authorization policies. SystemAdminOnly gates Module management
/// endpoints independently of any Action-code grant (design.md D1, spec:
/// action-code-authorization "System Admin Policy Gate for Module Management").
/// </summary>
public static class AuthorizationPolicies
{
    public const string SystemAdminOnly = "SystemAdminOnly";
}
