using FleetManager.Application.Vehicles.Errors;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Vehicles.DomainVehicles;
using FleetManager.Infrastructure.Persistence.Mysql.Context;

namespace FleetManager.Infrastructure.Persistence.Mysql.Vehicles.DomainService.Impl;

public class VehiclesDeleteService(DataBaseContext context) : IVehiclesDeleteService
{
    public async Task<Result<VoidResult, Error>> DeleteAsync(int idVehicle)
    {
        var entity = await context.Vehicles.FindAsync(idVehicle);
        if (entity == null)
            return VehiclesErrorBuilder.VehicleNotFound(idVehicle);

        context.Vehicles.Remove(entity);

        var result = await context.SaveChangesAsync() > 0;
        if (!result)
            return VehiclesErrorBuilder.VehicleDeleteException();

        return VoidResult.Instance;
    }
}
