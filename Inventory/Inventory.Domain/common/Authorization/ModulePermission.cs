namespace Inventory.Domain.Common.Authorization;

public record ModulePermission(
    string Module,
    bool CanView,
    bool CanCreate,
    bool CanEdit,
    bool CanDelete)
{
    /// <summary>
    /// Regla de negocio: el permiso sobre un módulo se deduce del SUFIJO de
    /// cada código de acción (Module.Name + verbo, ver 03_Actions_Seed.sql).
    /// Ej.: "UsersView" concede CanView sobre "Users".
    /// Antes esta regla estaba escondida en una consulta de Infrastructure.
    /// </summary>
    public static ModulePermission FromActionCodes(string module, IEnumerable<string> actionCodes)
    {
        var codes = actionCodes.ToList();

        return new ModulePermission(
            Module: module,
            CanView: codes.Any(c => c.EndsWith(PermissionAction.View, StringComparison.Ordinal)),
            CanCreate: codes.Any(c => c.EndsWith(PermissionAction.Create, StringComparison.Ordinal)),
            CanEdit: codes.Any(c => c.EndsWith(PermissionAction.Edit, StringComparison.Ordinal)),
            CanDelete: codes.Any(c => c.EndsWith(PermissionAction.Delete, StringComparison.Ordinal)));
    }
}
