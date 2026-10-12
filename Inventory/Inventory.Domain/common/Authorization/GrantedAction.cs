namespace Inventory.Domain.Common.Authorization;

/// <summary>
/// Dato plano que devuelve el repositorio: una acción concedida a un rol y el
/// módulo al que pertenece. No tiene reglas; las reglas están en
/// <see cref="ModulePermission.FromActionCodes"/>.
/// </summary>
public record GrantedAction(string ModuleName, string ActionCode);
