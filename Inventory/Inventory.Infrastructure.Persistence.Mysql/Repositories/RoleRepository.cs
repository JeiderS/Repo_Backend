using Inventory.Domain.Common.Authorization;
using Inventory.Domain.RoleActions.Entity;
using Inventory.Domain.Roles.Entity;
using Inventory.Domain.Roles.Repositories;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Repositories;

public class RoleRepository(DataBaseContext context) : IRoleRepository
{
    public async Task<RoleEntity?> GetByIdAsync(int id)
    {
        return await context.Roles.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IReadOnlyList<RoleEntity>> GetAllAsync()
    {
        return await context.Roles
            .OrderBy(r => r.Id)
            .ToListAsync();
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeRoleId = null)
    {
        return await context.Roles
            .AnyAsync(r => r.Name == name && (excludeRoleId == null || r.Id != excludeRoleId));
    }

    public async Task AddAsync(RoleEntity role)
    {
        await context.Roles.AddAsync(role);
    }

    public async Task<bool> IsSystemAdminAsync(int roleId)
    {
        return await context.Roles
            .Where(r => r.Id == roleId)
            .Select(r => (bool?)r.IsSystemAdmin)
            .FirstOrDefaultAsync() ?? false;
    }

    public async Task<IReadOnlyList<int>> GetActionIdsAsync(int roleId)
    {
        return await context.RoleActions
            .Where(ra => ra.RoleId == roleId)
            .Select(ra => ra.ActionId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetActionCodesAsync(int roleId)
    {
        // El claim de autorización es "{ModuleName sin espacios}{Action.Code}"
        // (ej. "Módulos" + "View" = "MódulosView" — Module.Name se guarda en
        // español, Action.Code en inglés), calculado acá y no
        // guardado en Actions.Code, para que Actions siga siendo un catálogo
        // genérico reutilizable (p.ej. "View" se crea una sola vez y se
        // asocia a cualquier módulo vía ModuleActions) sin romper los
        // [Authorize(Roles = "...")] existentes, que esperan el string
        // compuesto. Misma convención que Sql/TenantBootstrap/03_Actions_Seed.sql
        // (REPLACE(ModuleName, ' ', '') + Verb). Filtra por Module.IsActive y
        // Action.IsActive: un módulo o acción desactivados nunca deben seguir
        // otorgando permiso solo porque el RoleAction sigue existiendo.
        return await context.RoleActions
            .Where(ra => ra.RoleId == roleId
                && ra.ModuleAction.Module.IsActive
                && ra.ModuleAction.Action.IsActive)
            .Select(ra => ra.ModuleAction.Module.Name.Replace(" ", "") + ra.ModuleAction.Action.Code)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetActionCodesByModuleAsync(int roleId, int moduleId)
    {
        return await context.RoleActions
            .Where(ra => ra.RoleId == roleId
                && ra.ModuleAction.ModuleId == moduleId
                && ra.ModuleAction.Module.IsActive
                && ra.ModuleAction.Action.IsActive)
            .Select(ra => ra.ModuleAction.Module.Name.Replace(" ", "") + ra.ModuleAction.Action.Code)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<GrantedAction>> GetGrantedActionsAsync(int roleId)
    {
        return await context.RoleActions
            .Where(ra => ra.RoleId == roleId)
            .Select(ra => new GrantedAction(
                ra.ModuleAction.Module.Name,
                ra.ModuleAction.Action.Code))
            .ToListAsync();
    }

    public async Task<IReadOnlySet<int>> GetModuleIdsWithGrantedActionsAsync(int roleId)
    {
        var moduleIds = await context.RoleActions
            .Where(ra => ra.RoleId == roleId)
             .Select(ra => ra.ModuleId)
             .Distinct()
             .ToListAsync();
        return moduleIds.ToHashSet();
    }

    public async Task ReplaceActionsAsync(int roleId, IEnumerable<(int ModuleId, int ActionId)> grants)
    {
        var current = await context.RoleActions
            .Where(ra => ra.RoleId == roleId)
            .ToListAsync();

        context.RoleActions.RemoveRange(current);

        foreach (var grant in grants.Distinct())
        {
            await context.RoleActions.AddAsync(new RoleActionEntity
            {
                RoleId = roleId,
                ModuleId = grant.ModuleId,
                ActionId = grant.ActionId
            });
        }
    }

    public async Task<IReadOnlyDictionary<int, IReadOnlyList<(int ModuleId, int ActionId)>>> GetGrantedModuleActionsByRoleAsync()
    {
        var rows = await context.RoleActions
            .Select(ra => new { ra.RoleId, ra.ModuleId, ra.ActionId })
            .ToListAsync();

        return rows
            .GroupBy(r => r.RoleId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<(int ModuleId, int ActionId)>)g
                    .Select(r => (r.ModuleId, r.ActionId))
                    .ToList());
    }
}
