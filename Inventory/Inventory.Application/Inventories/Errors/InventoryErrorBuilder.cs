using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Inventories.Errors;

public static class InventoryErrorBuilder
{
    public static Error NotFound() =>
        Error.CreateInstance(
            "INVENTORY_NOT_FOUND",
            "El inventario especificado no existe.",
            ErrorType.NotFound);

    public static Error InsufficientStock() =>
        Error.CreateInstance(
            "INVENTORY_INSUFFICIENT_STOCK",
            "No hay suficientes existencias disponibles.",
            ErrorType.Conflict);
}
