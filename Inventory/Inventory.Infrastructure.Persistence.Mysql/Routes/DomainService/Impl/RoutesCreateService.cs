using Inventory.Application.Routes.Errors;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Routes.DomainRoutes;
using Inventory.Domain.Routes.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;

namespace Inventory.Infrastructure.Persistence.Mysql.Routes.DomainService.Impl;

public class RoutesCreateService(DataBaseContext context) : IRoutesCreateService
{
    public async Task<Result<VoidResult, Error>> CreateAsync(RoutesEntity routesEntity)
    {
        await context.Routes.AddAsync(routesEntity);

        var result = await context.SaveChangesAsync() > 0;
        if (!result)
            return RoutesErrorBuilder.RouteCreationException();
        return VoidResult.Instance;
    }
}
