using Inventory.Domain.Modules.DomainModules;
using Inventory.Domain.Modules.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Modules.DomainService.Impl;

public class ModuleGetByIdService(DataBaseContext context) : IModuleGetByIdService
{
    public async Task<ModuleEntity?> GetByIdAsync(int id)
    {
        return await context.Modules.FirstOrDefaultAsync(m => m.Id == id);
    }
}
