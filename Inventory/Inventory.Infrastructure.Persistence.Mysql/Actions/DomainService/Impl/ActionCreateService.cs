using Inventory.Domain.Actions.DomainActions;
using Inventory.Domain.Actions.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Actions.DomainService.Impl;

public class ActionCreateService(DataBaseContext context) : IActionCreateService
{
    public async Task<bool> CodeExistsAsync(string code)
    {
        return await context.Actions.AnyAsync(a => a.Code == code);
    }

    public async Task AddAsync(ActionEntity action)
    {
        await context.Actions.AddAsync(action);
    }
}
