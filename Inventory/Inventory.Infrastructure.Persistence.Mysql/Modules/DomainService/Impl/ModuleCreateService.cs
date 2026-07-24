using Inventory.Domain.Modules.DomainModules;
using Inventory.Domain.Modules.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Modules.DomainService.Impl;

public class ModuleCreateService(DataBaseContext context) : IModuleCreateService
{
    public async Task<bool> NameExistsAsync(string name)
    {
        return await context.Modules.AnyAsync(m => m.Name == name);
    }

    public async Task AddAsync(ModuleEntity module)
    {
        await context.Modules.AddAsync(module);
    }
}
