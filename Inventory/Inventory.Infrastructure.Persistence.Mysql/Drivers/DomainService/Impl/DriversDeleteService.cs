using Inventory.Application.Drivers.Errors;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Drivers.DomainDrivers;
using Inventory.Infrastructure.Persistence.Mysql.Context;

namespace Inventory.Infrastructure.Persistence.Mysql.Drivers.DomainService.Impl;

public class DriversDeleteService(DataBaseContext context) : IDriversDeleteService
{
    public async Task<Result<VoidResult, Error>> DeleteAsync(int idDrivers)
    {
        var entity = await context.Drivers.FindAsync(idDrivers);
        if (entity == null)
            return DriversErrorBuilder.DriverNotFound(idDrivers);

        context.Drivers.Remove(entity);

        var result = await context.SaveChangesAsync() > 0;
        if (!result)
            return DriversErrorBuilder.DriverDeleteException();

        return VoidResult.Instance;
    }
}
