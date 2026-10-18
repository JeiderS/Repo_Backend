namespace Inventory.Application.Roles.Dto;

/// <summary>Una concesión puntual: el rol tiene la acción <see cref="ActionId"/>
/// del módulo <see cref="ModuleId"/>. Reemplaza al antiguo "ActionId suelto"
/// porque, con Actions como catálogo global reutilizable vía ModuleActions,
/// un ActionId por sí solo ya no identifica a qué módulo pertenece el
/// permiso concedido.</summary>
public sealed record RoleModuleActionGrant(int ModuleId, int ActionId);
