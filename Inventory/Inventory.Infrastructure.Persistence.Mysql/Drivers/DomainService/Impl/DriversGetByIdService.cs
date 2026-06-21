using Microsoft.EntityFrameworkCore;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Inventory.Domain.Drivers.DomainDrivers;
using Inventory.Domain.Drivers.Entity;
using Inventory.Application.Drivers.Errors;

namespace Inventory.Infrastructure.Persistence.Mysql.Drivers.DomainService.Impl;

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