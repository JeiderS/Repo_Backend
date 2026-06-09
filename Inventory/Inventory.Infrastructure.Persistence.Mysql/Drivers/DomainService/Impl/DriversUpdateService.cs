using Microsoft.EntityFrameworkCore;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Infrastructure.Persistence.Mysql.Context;
using FleetManager.Domain.Drivers.DomainDrivers;
using FleetManager.Domain.Drivers.Entity;
using FleetManager.Application.Drivers.Errors;

namespace FleetManager.Infrastructure.Persistence.Mysql.Drivers.DomainService.Impl;

public class DriversUpdateService(DataBaseContext context) : IDriversUpdateService
{
    public async Task<Result<VoidResult, Error>> UpdateAsync(DriversEntity DriversEntity)
    {
        if (!await EntityExists(DriversEntity.Id))
            return DriversErrorBuilder.DriverNotFoundException(DriversEntity.Id);

        context.Drivers.Update(DriversEntity);

        if (await context.SaveChangesAsync() <= 0)
            return DriversErrorBuilder.DriverUpdateException();
        return VoidResult.Instance;
    }
    private async Task<bool> EntityExists(int id)
    {
        return await context.Drivers
            .AsNoTracking()
            .AnyAsync(c => c.Id == id);
    }
}