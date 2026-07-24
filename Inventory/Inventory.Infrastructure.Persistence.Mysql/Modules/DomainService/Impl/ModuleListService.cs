using Inventory.Domain.Modules.DomainModules;
using Inventory.Domain.Modules.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Modules.DomainService.Impl;

public class ModuleListService(DataBaseContext context) : IModuleListService
{
    public async Task<IReadOnlyList<ModuleEntity>> GetAllAsync()
    {
        return await context.Modules
            .OrderBy(m => m.SortOrder)
            .ToListAsync();
    }
}
