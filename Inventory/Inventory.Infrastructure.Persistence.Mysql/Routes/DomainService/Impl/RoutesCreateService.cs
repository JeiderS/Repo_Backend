using FleetManager.Application.Routes.Errors;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Routes.DomainRoutes;
using FleetManager.Domain.Routes.Entity;
using FleetManager.Infrastructure.Persistence.Mysql.Context;

namespace FleetManager.Infrastructure.Persistence.Mysql.Routes.DomainService.Impl;

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
