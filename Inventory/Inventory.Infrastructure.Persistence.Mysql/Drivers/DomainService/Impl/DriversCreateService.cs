using Inventory.Application.Drivers.Errors;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Drivers.DomainDrivers;
using Inventory.Domain.Drivers.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;

namespace Inventory.Infrastructure.Persistence.Mysql.Drivers.DomainService.Impl;

public class DriversCreateService(DataBaseContext context) : IDriversCreateService
{
    public async Task<Result<VoidResult, Error>> CreateAsync(DriversEntity DriversEntity)
    {
        await context.Drivers.AddAsync(DriversEntity);

        var result = await context.SaveChangesAsync() > 0;
        if (!result)
            return DriversErrorBuilder.DriverCreationException();
        return VoidResult.Instance;
    }
}