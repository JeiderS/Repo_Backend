using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Domain.Modules;

/// <summary>
/// Errores que nacen de las reglas propias del módulo (no de la base de datos).
/// Viven en Domain porque es la entidad la que los detecta.
/// </summary>
public static class ModuleErrors
{
    public static Error CannotBeItsOwnParent() =>
        Error.CreateInstance(
            "MODULE_INVALID_PARENT",
            "Un módulo no puede ser padre de sí mismo.",
            ErrorType.Validation);
}
