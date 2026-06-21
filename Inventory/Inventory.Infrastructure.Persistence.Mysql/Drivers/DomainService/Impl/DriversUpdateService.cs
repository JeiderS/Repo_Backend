using Microsoft.EntityFrameworkCore;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Inventory.Domain.Drivers.DomainDrivers;
using Inventory.Domain.Drivers.Entity;
using Inventory.Application.Drivers.Errors;

namespace Inventory.Infrastructure.Persistence.Mysql.Drivers.DomainService.Impl;

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