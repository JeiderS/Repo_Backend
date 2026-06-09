using Microsoft.EntityFrameworkCore;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Infrastructure.Persistence.Mysql.Context;
using FleetManager.Domain.Vehicles.DomainVehicles;
using FleetManager.Domain.Vehicles.Entity;
using FleetManager.Application.Vehicles.Errors;

namespace FleetManager.Infrastructure.Persistence.Mysql.Vehicles.DomainService.Impl;

public class VehiclesGetByIdService(DataBaseContext context) : IVehiclesGetByIdService
{
    public async Task<Result<VehiclesEntity, Error>> GetByIdAsync(int id)
    {
        if (!await EntityExists(id))
            return VehiclesErrorBuilder.VehicleNotFoundException(id);

        var data = await context.Vehicles
            .FirstAsync(c => c.Id == id);

        return data;
    }

    private async Task<bool> EntityExists(int id)
    {
        return await context.Vehicles
            .AsNoTracking()
            .AnyAsync(c => c.Id == id);
    }
}
