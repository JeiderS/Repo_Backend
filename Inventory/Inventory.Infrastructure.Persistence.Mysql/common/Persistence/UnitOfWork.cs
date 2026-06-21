using Inventory.Domain.Common.Persistence;
using Inventory.Infrastructure.Persistence.Mysql.Context;

namespace Inventory.Infrastructure.Persistence.Mysql.Common.Persistence;

public class UnitOfWork(DataBaseContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
