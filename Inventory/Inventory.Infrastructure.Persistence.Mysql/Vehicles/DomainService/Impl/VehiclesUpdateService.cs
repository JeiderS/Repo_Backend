using Microsoft.EntityFrameworkCore;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Inventory.Domain.Vehicles.DomainVehicles;
using Inventory.Domain.Vehicles.Entity;
using Inventory.Application.Vehicles.Errors;

namespace Inventory.Infrastructure.Persistence.Mysql.Vehicles.DomainService.Impl;

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
