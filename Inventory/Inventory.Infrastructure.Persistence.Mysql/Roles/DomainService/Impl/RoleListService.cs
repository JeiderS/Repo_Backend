using Inventory.Domain.Roles.DomainRoles;
using Inventory.Domain.Roles.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Roles.DomainService.Impl;

public class RoleListService(DataBaseContext context) : IRoleListService
{
    public async Task<IReadOnlyList<RoleEntity>> GetAllAsync()
    {
        return await context.Roles
            .OrderBy(r => r.Id)
            .ToListAsync();
    }
}
