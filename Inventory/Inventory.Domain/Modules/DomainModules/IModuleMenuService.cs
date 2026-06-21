using Inventory.Domain.Modules.Models;

namespace Inventory.Domain.Modules.DomainModules;

public interface IModuleMenuService
{
    /// <summary>
    /// Construye el árbol de módulos visibles para el usuario: un módulo
    /// aparece si tiene permiso CanView (vía sus roles) o si alguno de sus
    /// descendientes es visible (para no ocultar contenedores intermedios).
    /// </summary>
    Task<IReadOnlyList<ModuleMenuItem>> GetMenuForUserAsync(int userId);
}
