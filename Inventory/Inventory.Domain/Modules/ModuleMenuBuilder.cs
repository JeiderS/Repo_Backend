using Inventory.Domain.Modules.Entity;
using Inventory.Domain.Modules.Models;

namespace Inventory.Domain.Modules;

/// <summary>
/// Servicio de dominio: arma el árbol del menú a partir de datos que ya
/// están en memoria. Es una regla de negocio que no cabe en UNA sola entidad
/// (necesita ver todos los módulos a la vez), por eso es un servicio de
/// dominio y no un método de ModuleEntity. No toca la base de datos: se
/// puede probar con datos en memoria.
/// </summary>
public static class ModuleMenuBuilder
{
    // ModuleId es IDENTITY(1,1), nunca vale 0; lo usamos como clave de "raíz"
    // para evitar una clave nullable en el diccionario de agrupación.
    private const int RootKey = 0;

    /// <param name="activeModules">Módulos activos, ya ordenados por SortOrder.</param>
    /// <param name="moduleIdsWithGrantedActions">Módulos en los que el rol tiene al menos una acción.</param>
    /// <param name="roleIsSystemAdmin">Si el rol del usuario es System Admin.</param>
    public static IReadOnlyList<ModuleMenuItem> Build(
        IReadOnlyList<ModuleEntity> activeModules,
        IReadOnlySet<int> moduleIdsWithGrantedActions,
        bool roleIsSystemAdmin)
    {
        var permittedModuleIds = activeModules
            .Where(m => m.IsVisibleTo(moduleIdsWithGrantedActions.Contains(m.Id), roleIsSystemAdmin))
            .Select(m => m.Id)
            .ToHashSet();

        var modulesByParent = activeModules
            .GroupBy(m => m.ParentId ?? RootKey)
            .ToDictionary(g => g.Key, g => g.ToList());

        return BuildVisibleLevel(RootKey, modulesByParent, permittedModuleIds);
    }

    private static List<ModuleMenuItem> BuildVisibleLevel(
        int parentKey,
        IReadOnlyDictionary<int, List<ModuleEntity>> modulesByParent,
        HashSet<int> permittedModuleIds)
    {
        if (!modulesByParent.TryGetValue(parentKey, out var siblings))
            return [];

        var result = new List<ModuleMenuItem>();

        foreach (var module in siblings)
        {
            var children = BuildVisibleLevel(module.Id, modulesByParent, permittedModuleIds);

            // Regla: un módulo se muestra si el rol puede verlo, o si alguno de
            // sus descendientes es visible (no ocultar contenedores).
            var isVisible = permittedModuleIds.Contains(module.Id) || children.Count > 0;
            if (!isVisible)
                continue;

            result.Add(new ModuleMenuItem(module.Name, module.Icon, module.Route, children));
        }

        return result;
    }
}
