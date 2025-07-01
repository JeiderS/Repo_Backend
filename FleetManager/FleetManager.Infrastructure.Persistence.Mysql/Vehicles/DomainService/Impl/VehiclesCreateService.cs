using FleetManager.Application.Vehicles.Errors;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Vehicles.DomainVehicles;
using FleetManager.Domain.Vehicles.Entity;
using FleetManager.Infrastructure.Persistence.Mysql.Context;

namespace FleetManager.Infrastructure.Persistence.Mysql.Vehicles.DomainService.Impl;

public class VehiclesCreateService(DataBaseContext context) : IVehiclesCreateService
{
    public async Task<Result<VoidResult, Error>> CreateAsync(VehiclesEntity vehiclesEntity)
    {
        await context.Vehicles.AddAsync(vehiclesEntity);

        var result = await context.SaveChangesAsync() > 0;
        if (!result)
            return VehiclesErrorBuilder.VehicleCreationException();
        return VoidResult.Instance;
    }
}
