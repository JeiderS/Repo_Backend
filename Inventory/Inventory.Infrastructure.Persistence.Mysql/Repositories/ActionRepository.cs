using Inventory.Domain.Actions.Entity;
using Inventory.Domain.Actions.Repositories;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Repositories;

public class ActionRepository(DataBaseContext context) : IActionRepository
{
    public async Task<IReadOnlyList<ActionEntity>> GetAllAsync()
    {
        return await context.Actions
            .Include(a => a.Module)
            .OrderBy(a => a.Id)
            .ToListAsync();
    }

    public async Task<bool> CodeExistsAsync(string code)
    {
        return await context.Actions.AnyAsync(a => a.Code == code);
    }

    public async Task AddAsync(ActionEntity action)
    {
        await context.Actions.AddAsync(action);
    }

    public async Task<IReadOnlyList<int>> GetExistingIdsAsync(IEnumerable<int> actionIds)
    {
        var ids = actionIds.ToList();
        return await context.Actions
            .Where(a => ids.Contains(a.Id))
            .Select(a => a.Id)
            .ToListAsync();
    }
}
