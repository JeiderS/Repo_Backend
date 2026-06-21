using Inventory.Application.Vehicles.Errors;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Vehicles.DomainVehicles;
using Inventory.Domain.Vehicles.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;

namespace Inventory.Infrastructure.Persistence.Mysql.Vehicles.DomainService.Impl;

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
