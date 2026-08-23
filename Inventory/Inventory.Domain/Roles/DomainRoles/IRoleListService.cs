using Inventory.Domain.Roles.Entity;

namespace Inventory.Domain.Roles.DomainRoles;

public interface IRoleListService
{
    Task<IReadOnlyList<RoleEntity>> GetAllAsync();
}
