using Inventory.Domain.Modules.Entity;

namespace Inventory.Domain.Modules.Repositories;

/// <summary>
/// Contrato para módulos. Reemplaza a IModuleCreateService,
/// IModuleGetByIdService, IModuleListService e IModuleUpdateService.
/// </summary>
public interface IModuleRepository
{
    Task<ModuleEntity?> GetByIdAsync(int id);

    /// <summary>Busca un módulo por su Route (slug estable), o null si no existe.</summary>
    Task<ModuleEntity?> GetByRouteAsync(string route);

    Task<bool> ExistsAsync(int id);

    /// <summary>Todos los módulos, ordenados por SortOrder.</summary>
    Task<IReadOnlyList<ModuleEntity>> GetAllAsync();

    /// <summary>Solo los módulos activos, ordenados por SortOrder.</summary>
    Task<IReadOnlyList<ModuleEntity>> GetActiveAsync();

    /// <summary>¿Ya hay un módulo con ese nombre? (ignorando <paramref name="excludeModuleId"/>).</summary>
    Task<bool> NameExistsAsync(string name, int? excludeModuleId = null);

    Task AddAsync(ModuleEntity module);

    /// <summary>Ids de las acciones asignadas a un módulo (tabla ModuleActions).</summary>
    Task<IReadOnlyList<int>> GetActionIdsAsync(int moduleId);

    /// <summary>Ids de las acciones asignadas, agrupados por módulo, en una sola consulta (evita N+1).</summary>
    Task<IReadOnlyDictionary<int, IReadOnlyList<int>>> GetActionIdsByModuleAsync();

    /// <summary>
    /// De los <paramref name="actionIds"/> indicados, cuáles están en uso por
    /// al menos un rol (RoleActions) para este módulo. Se usa para impedir
    /// que se quite silenciosamente una acción que un rol todavía tiene asignada.
    /// </summary>
    Task<IReadOnlySet<int>> GetActionIdsInUseByRolesAsync(int moduleId, IEnumerable<int> actionIds);

    /// <summary>Agrega relaciones Módulo-Acción. No hace commit (ver IUnitOfWork).</summary>
    Task AddModuleActionsAsync(int moduleId, IEnumerable<int> actionIds);

    /// <summary>Quita relaciones Módulo-Acción. No hace commit (ver IUnitOfWork).</summary>
    Task RemoveModuleActionsAsync(int moduleId, IEnumerable<int> actionIds);
}
