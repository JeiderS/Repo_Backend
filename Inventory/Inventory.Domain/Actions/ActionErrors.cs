using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Domain.Actions;

public static class ActionErrors
{
    public static Error InvalidName() =>
        Error.CreateInstance(
            "ACTION_INVALID_NAME",
            "El nombre de la acción no es válido.",
            ErrorType.Validation);
}
