namespace Inventory.Domain.Common.Authorization;

public record ModulePermission(
    string Module,
    bool CanView,
    bool CanCreate,
    bool CanEdit,
    bool CanDelete);
