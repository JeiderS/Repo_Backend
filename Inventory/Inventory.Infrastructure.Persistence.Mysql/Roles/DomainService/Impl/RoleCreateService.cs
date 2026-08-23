using Inventory.Domain.Roles.DomainRoles;
using Inventory.Domain.Roles.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Roles.DomainService.Impl;

public class RoleCreateService(DataBaseContext context) : IRoleCreateService
{
    public async Task<bool> NameExistsAsync(string name)
    {
        return await context.Roles.AnyAsync(r => r.Name == name);
    }

    public async Task AddAsync(RoleEntity role)
    {
        await context.Roles.AddAsync(role);
    }
}
