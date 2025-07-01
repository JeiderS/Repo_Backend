using FleetManager.Application.Drivers.Errors;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Drivers.DomainDrivers;
using FleetManager.Domain.Drivers.Entity;
using FleetManager.Infrastructure.Persistence.Mysql.Context;

namespace FleetManager.Infrastructure.Persistence.Mysql.Drivers.DomainService.Impl;

public class DriversCreateService(DataBaseContext context) : IDriversCreateService
{
    public async Task<Result<VoidResult, Error>> CreateAsync(DriversEntity DriversEntity)
    {
        await context.Drivers.AddAsync(DriversEntity);

        var result = await context.SaveChangesAsync() > 0;
        if (!result)
            return DriversErrorBuilder.DriverCreationException();
        return VoidResult.Instance;
    }
}