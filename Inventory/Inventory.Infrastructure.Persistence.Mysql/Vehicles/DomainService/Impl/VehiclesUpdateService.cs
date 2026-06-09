using Microsoft.EntityFrameworkCore;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Infrastructure.Persistence.Mysql.Context;
using FleetManager.Domain.Vehicles.DomainVehicles;
using FleetManager.Domain.Vehicles.Entity;
using FleetManager.Application.Vehicles.Errors;

namespace FleetManager.Infrastructure.Persistence.Mysql.Vehicles.DomainService.Impl;

public class VehiclesUpdateService(DataBaseContext context) : IVehiclesUpdateService
{
    public async Task<Result<VoidResult, Error>> UpdateAsync(VehiclesEntity vehiclesEntity)
    {
        if (!await EntityExists(vehiclesEntity.Id))
            return VehiclesErrorBuilder.VehicleNotFoundException(vehiclesEntity.Id);

        context.Vehicles.Update(vehiclesEntity);

        if (await context.SaveChangesAsync() <= 0)
            return VehiclesErrorBuilder.VehicleUpdateException();

        return VoidResult.Instance;
    }

    private async Task<bool> EntityExists(int id)
    {
        return await context.Vehicles
            .AsNoTracking()
            .AnyAsync(c => c.Id == id);
    }
}
