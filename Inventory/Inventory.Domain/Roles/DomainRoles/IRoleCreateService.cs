using Inventory.Domain.Roles.Entity;

namespace Inventory.Domain.Roles.DomainRoles;

public interface IRoleCreateService
{
    Task<bool> NameExistsAsync(string name);
    Task AddAsync(RoleEntity role);
}
