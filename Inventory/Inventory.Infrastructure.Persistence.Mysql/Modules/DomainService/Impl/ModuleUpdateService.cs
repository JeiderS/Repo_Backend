using Inventory.Domain.Modules.DomainModules;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Modules.DomainService.Impl;

public class ModuleUpdateService(DataBaseContext context) : IModuleUpdateService
{
    public async Task<bool> NameExistsForOtherAsync(string name, int excludeId)
    {
        return await context.Modules.AnyAsync(m => m.Name == name && m.Id != excludeId);
    }
}
