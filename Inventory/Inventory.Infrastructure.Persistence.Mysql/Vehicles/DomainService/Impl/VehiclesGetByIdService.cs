using Microsoft.EntityFrameworkCore;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Inventory.Domain.Vehicles.DomainVehicles;
using Inventory.Domain.Vehicles.Entity;
using Inventory.Application.Vehicles.Errors;

namespace Inventory.Infrastructure.Persistence.Mysql.Vehicles.DomainService.Impl;

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
