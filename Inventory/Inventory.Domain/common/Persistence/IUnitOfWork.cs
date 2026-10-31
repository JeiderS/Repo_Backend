namespace Inventory.Domain.Common.Persistence;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Ejecuta <paramref name="operation"/> dentro de una transacción de base
    /// de datos: si falla, revierte todo lo que haya hecho (incluyendo los
    /// SaveChangesAsync intermedios). Usar solo cuando un caso de uso
    /// necesita que varias operaciones de escritura se confirmen juntas o
    /// ninguna lo haga (ej. actualizar Inventory e insertar su Movement).
    /// </summary>
    Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation);
}
