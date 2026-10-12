using Inventory.Domain.Modules.Entity;

namespace Inventory.Domain.Modules.Repositories;

/// <summary>
/// Contrato para módulos. Reemplaza a IModuleCreateService,
/// IModuleGetByIdService, IModuleListService e IModuleUpdateService.
/// </summary>
public interface IModuleRepository
{
    Task<ModuleEntity?> GetByIdAsync(int id);

    Task<bool> ExistsAsync(int id);

    /// <summary>Todos los módulos, ordenados por SortOrder.</summary>
    Task<IReadOnlyList<ModuleEntity>> GetAllAsync();

    /// <summary>Solo los módulos activos, ordenados por SortOrder.</summary>
    Task<IReadOnlyList<ModuleEntity>> GetActiveAsync();

    /// <summary>¿Ya hay un módulo con ese nombre? (ignorando <paramref name="excludeModuleId"/>).</summary>
    Task<bool> NameExistsAsync(string name, int? excludeModuleId = null);

    Task AddAsync(ModuleEntity module);
}
