using Inventory.Domain.Roles.Entity;

namespace Inventory.Domain.Roles.DomainRoles;

public interface IRoleGetByIdService
{
    Task<RoleEntity?> GetByIdAsync(int id);
}
