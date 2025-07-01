using FleetManager.Application.Drivers.Errors;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Drivers.DomainDrivers;
using FleetManager.Infrastructure.Persistence.Mysql.Context;

namespace FleetManager.Infrastructure.Persistence.Mysql.Drivers.DomainService.Impl;

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
