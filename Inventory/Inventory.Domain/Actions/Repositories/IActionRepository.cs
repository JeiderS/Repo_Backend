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

    Task<ActionEntity?> GetByIdAsync(int id);

    /// <summary>¿Ya hay otra acción con ese código? (ignorando <paramref name="excludeActionId"/>).</summary>
    Task<bool> CodeExistsAsync(string code, int? excludeActionId = null);

    Task AddAsync(ActionEntity action);

    Task RemoveAsync(ActionEntity action);

    /// <summary>De los ids recibidos, devuelve solo los que existen en el catálogo.</summary>
    Task<IReadOnlyList<int>> GetExistingIdsAsync(IEnumerable<int> actionIds);

    /// <summary>¿Esta acción está asignada a al menos un módulo (ModuleActions)?
    /// Si no, tampoco puede estar concedida a ningún rol (RoleActions depende
    /// de esa relación vía FK), así que es seguro eliminarla.</summary>
    Task<bool> IsAssignedToAnyModuleAsync(int actionId);
}
