using Inventory.Domain.Modules.DomainModules;
using Inventory.Domain.Modules.Entity;
using Inventory.Domain.Modules.Models;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Modules.DomainService.Impl;

public class ModuleMenuService(DataBaseContext context) : IModuleMenuService
{
    // ModuleId es IDENTITY(1,1), nunca vale 0; lo usamos como clave de "raíz"
    // para evitar una clave nullable en el diccionario de agrupación.
    private const int RootKey = 0;

    public async Task<IReadOnlyList<ModuleMenuItem>> GetMenuForUserAsync(int userId)
    {
        var roleIds = await context.Users
            .Where(u => u.Id == userId && u.RoleId != null)
            .Select(u => u.RoleId!.Value)
            .ToListAsync();

        var allModules = await context.Modules
            .Where(m => m.IsActive)
            .OrderBy(m => m.SortOrder)
            .ToListAsync();

        var permittedModuleIds = roleIds.Count == 0
            ? new HashSet<int>()
            : (await context.RoleModules
                .Where(rm => roleIds.Contains(rm.RoleId) && rm.CanView)
                .Select(rm => rm.ModuleId)
                .ToListAsync())
              .ToHashSet();

        var modulesByParent = allModules
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

            // Un módulo se muestra si el usuario tiene CanView sobre él, o si
            // alguno de sus descendientes es visible (no ocultar contenedores).
            var isVisible = permittedModuleIds.Contains(module.Id) || children.Count > 0;
            if (!isVisible)
                continue;

            result.Add(new ModuleMenuItem(module.Name, module.Icon, module.Route, children));
        }

        return result;
    }
}
