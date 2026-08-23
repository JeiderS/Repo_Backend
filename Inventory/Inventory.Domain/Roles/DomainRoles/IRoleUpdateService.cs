namespace Inventory.Domain.Roles.DomainRoles;

public interface IRoleUpdateService
{
    Task<bool> NameExistsForOtherAsync(string name, int excludeId);
}
