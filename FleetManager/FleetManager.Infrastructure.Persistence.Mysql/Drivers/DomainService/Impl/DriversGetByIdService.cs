using Microsoft.EntityFrameworkCore;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Infrastructure.Persistence.Mysql.Context;
using FleetManager.Domain.Drivers.DomainDrivers;
using FleetManager.Domain.Drivers.Entity;
using FleetManager.Application.Drivers.Errors;

namespace FleetManager.Infrastructure.Persistence.Mysql.Drivers.DomainService.Impl;

public class DriversGetByIdService(DataBaseContext context) : IDriversGetByIdService
   {
    public async Task<Result<DriversEntity, Error>> GetByIdAsync(int id)
    {

        if (!await EntityExists(id))
            return DriversErrorBuilder.DriverNotFoundException(id);

        var data = await context.Drivers
            .FirstAsync(c => c.Id == id);

        return data;
    }

    private async Task<bool> EntityExists(int id)
    {

        return await context.Drivers
            .AsNoTracking()
            .AnyAsync(c => c.Id == id);
}
   }