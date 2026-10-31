namespace Inventory.Domain.Common.Persistence;

/// <summary>
/// Se lanza dentro de una venta multi-línea cuando, al momento de aplicar la
/// salida real (segunda pasada, ya dentro de la transacción), el stock de un
/// producto ya no es suficiente -- por ejemplo, otra venta concurrente lo
/// consumió entre la validación optimista de la primera pasada y este punto.
///
/// Lanzarla (en vez de devolver un Result.Failure) es intencional: dispara el
/// rollback de IUnitOfWork.ExecuteInTransactionAsync, revirtiendo la Sale ya
/// insertada y cualquier línea anterior de esta misma venta que ya haya
/// decrementado inventario. Vive en Domain junto a IUnitOfWork, misma razón
/// que DuplicateRecordException/ConcurrencyConflictException.
/// </summary>
public class InsufficientStockException(string productName)
    : Exception($"No hay suficientes existencias de '{productName}' para completar la venta.")
{
    public string ProductName { get; } = productName;
}
