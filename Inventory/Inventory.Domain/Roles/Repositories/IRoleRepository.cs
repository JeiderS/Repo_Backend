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

    /// <summary>
    /// Ids de acciones de TODOS los roles en una sola consulta (roleId -> actionIds).
    /// Evita el problema "N+1": antes se hacía una consulta por cada rol.
    /// </summary>
    Task<IReadOnlyDictionary<int, IReadOnlyList<int>>> GetActionIdsByRoleAsync();

    /// <summary>Códigos de acción (ej. "UsersView") concedidos al rol.</summary>
    Task<IReadOnlyList<string>> GetActionCodesAsync(int roleId);

    /// <summary>Acciones concedidas al rol, cada una con el nombre de su módulo.</summary>
    Task<IReadOnlyList<GrantedAction>> GetGrantedActionsAsync(int roleId);

    /// <summary>Ids de los módulos en los que el rol tiene al menos una acción.</summary>
    Task<IReadOnlySet<int>> GetModuleIdsWithGrantedActionsAsync(int roleId);

    /// <summary>Reemplazo total: borra las acciones actuales del rol y pone las nuevas.</summary>
    Task ReplaceActionsAsync(int roleId, IEnumerable<int> actionIds);
}
