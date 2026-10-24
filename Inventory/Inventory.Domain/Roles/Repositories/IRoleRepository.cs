using Inventory.Domain.Common.Authorization;
using Inventory.Domain.Roles.Entity;

namespace Inventory.Domain.Roles.Repositories;

/// <summary>
/// Contrato para roles y sus acciones asignadas. Reemplaza a IRoleCreateService,
/// IRoleGetByIdService, IRoleListService, IRoleUpdateService e
/// IRoleActionAssignService.
/// </summary>
public interface IRoleRepository
{
    Task<RoleEntity?> GetByIdAsync(int id);

    Task<IReadOnlyList<RoleEntity>> GetAllAsync();

    /// <summary>¿Ya hay un rol con ese nombre? (ignorando <paramref name="excludeRoleId"/>).</summary>
    Task<bool> NameExistsAsync(string name, int? excludeRoleId = null);

    Task AddAsync(RoleEntity role);

    Task<bool> IsSystemAdminAsync(int roleId);

    Task<IReadOnlyList<int>> GetActionIdsAsync(int roleId);

    /// <summary>Códigos de acción (ej. "UsersView") concedidos al rol.</summary>
    Task<IReadOnlyList<string>> GetActionCodesAsync(int roleId);

    /// <summary>Códigos de acción concedidos al rol, pero solo del módulo indicado
    /// (contexto dinámico vía header X-Module-Code).</summary>
    Task<IReadOnlyList<string>> GetActionCodesByModuleAsync(int roleId, int moduleId);

    /// <summary>Acciones concedidas al rol, cada una con el nombre de su módulo.</summary>
    Task<IReadOnlyList<GrantedAction>> GetGrantedActionsAsync(int roleId);

    /// <summary>Ids de los módulos en los que el rol tiene al menos una acción.</summary>
    Task<IReadOnlySet<int>> GetModuleIdsWithGrantedActionsAsync(int roleId);

    /// <summary>Reemplazo total: borra las concesiones actuales del rol y pone las
    /// nuevas. Se usa la tupla (ModuleId, ActionId) en vez del record
    /// RoleModuleActionGrant de Application.Roles.Dto porque Domain no puede
    /// referenciar Application (evitaría una dependencia circular de
    /// proyectos: Application ya referencia Domain).</summary>
    Task ReplaceActionsAsync(int roleId, IEnumerable<(int ModuleId, int ActionId)> grants);

    /// <summary>Concesiones (ModuleId, ActionId) de TODOS los roles en una sola
    /// consulta (roleId -> grants). Evita el problema "N+1".</summary>
    Task<IReadOnlyDictionary<int, IReadOnlyList<(int ModuleId, int ActionId)>>> GetGrantedModuleActionsByRoleAsync();
}
