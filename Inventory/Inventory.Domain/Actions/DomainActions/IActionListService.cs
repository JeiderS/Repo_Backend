using Inventory.Domain.Actions.Entity;

namespace Inventory.Domain.Actions.DomainActions;

public interface IActionListService
{
    Task<IReadOnlyList<ActionEntity>> GetAllAsync();
}
