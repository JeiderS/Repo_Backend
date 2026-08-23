using Inventory.Domain.Actions.DomainActions;
using Inventory.Domain.Actions.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Actions.DomainService.Impl;

public class ActionListService(DataBaseContext context) : IActionListService
{
    public async Task<IReadOnlyList<ActionEntity>> GetAllAsync()
    {
        return await context.Actions
            .Include(a => a.Module)
            .OrderBy(a => a.Id)
            .ToListAsync();
    }
}
