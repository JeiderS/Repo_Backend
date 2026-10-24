namespace Inventory.Domain.Common.Persistence;

/// <summary>
/// Se lanza cuando otra petición ya modificó la misma fila mientras esta
/// estaba en curso (ej. dos requests rotando el mismo refresh token a la
/// vez). Vive en Domain junto a IUnitOfWork, por la misma razón que
/// <see cref="DuplicateRecordException"/>: Infrastructure TRADUCE su
/// excepción técnica (DbUpdateConcurrencyException) a esta, para que nadie
/// fuera de Infrastructure tenga que conocer EF Core.
/// </summary>
public class ConcurrencyConflictException(string message, Exception innerException)
    : Exception(message, innerException);
