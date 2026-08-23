using Inventory.Domain.Roles.DomainRoles;
using Inventory.Domain.Roles.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Roles.DomainService.Impl;

public class RoleGetByIdService(DataBaseContext context) : IRoleGetByIdService
{
    public async Task<RoleEntity?> GetByIdAsync(int id)
    {
        return await context.Roles.FirstOrDefaultAsync(r => r.Id == id);
    }
}
