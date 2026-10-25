using Inventory.Domain.Common.Persistence;
using Inventory.Infrastructure.Persistence.SqlServer.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.SqlServer.Common.Persistence;

public class UnitOfWork(DataBaseContext context) : IUnitOfWork
{
    // Códigos de SQL Server para "violación de índice único / clave única".
    private const int UniqueIndexViolation = 2601;
    private const int UniqueConstraintViolation = 2627;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException
        {
            Number: UniqueIndexViolation or UniqueConstraintViolation
        })
        {
            // Traducimos el error técnico de SQL Server a uno que entiende el
            // resto del sistema. La Api lo convierte en un 409 Conflict.
            throw new DuplicateRecordException("Ya existe un registro con esos datos.", ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Alguien más ya guardó cambios sobre la misma fila (ej. el mismo
            // refresh token rotado por dos requests a la vez). Quien llamó
            // decide qué significa eso para su caso de uso.
            throw new ConcurrencyConflictException("El registro fue modificado por otra operación.", ex);
        }
    }
}
