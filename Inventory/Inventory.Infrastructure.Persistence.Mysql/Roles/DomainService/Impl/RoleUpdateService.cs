using Inventory.Domain.Roles.DomainRoles;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Roles.DomainService.Impl;

public class RoleUpdateService(DataBaseContext context) : IRoleUpdateService
{
    public async Task<bool> NameExistsForOtherAsync(string name, int excludeId)
    {
        return await context.Roles.AnyAsync(r => r.Name == name && r.Id != excludeId);
    }
}
