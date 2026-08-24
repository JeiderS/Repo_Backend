using Inventory.Domain.Actions.Entity;

namespace Inventory.Domain.Actions.DomainActions;

public interface IActionCreateService
{
    Task<bool> CodeExistsAsync(string code);
    Task AddAsync(ActionEntity action);
}
