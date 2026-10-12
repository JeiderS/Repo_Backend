using Inventory.Domain.Actions.Entity;

namespace Inventory.Domain.Actions.Repositories;

/// <summary>
/// Contrato para el catálogo de acciones. Reemplaza a IActionCreateService e
/// IActionListService.
/// </summary>
public interface IActionRepository
{
    /// <summary>Todas las acciones, con su módulo.</summary>
    Task<IReadOnlyList<ActionEntity>> GetAllAsync();

    Task<bool> CodeExistsAsync(string code);

    Task AddAsync(ActionEntity action);

    /// <summary>De los ids recibidos, devuelve solo los que existen en el catálogo.</summary>
    Task<IReadOnlyList<int>> GetExistingIdsAsync(IEnumerable<int> actionIds);
}
