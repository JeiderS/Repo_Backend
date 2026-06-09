using Microsoft.EntityFrameworkCore;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Infrastructure.Persistence.Mysql.Context;
using FleetManager.Domain.Routes.DomainRoutes;
using FleetManager.Domain.Routes.Entity;
using FleetManager.Application.Routes.Errors;

namespace FleetManager.Infrastructure.Persistence.Mysql.Routes.DomainService.Impl;

public class RoutesUpdateService(DataBaseContext context) : IRoutesUpdateService
{
    public async Task<Result<VoidResult, Error>> UpdateAsync(RoutesEntity routesEntity)
    {
        if (!await EntityExists(routesEntity.Id))
            return RoutesErrorBuilder.RouteNotFoundException(routesEntity.Id);

        context.Routes.Update(routesEntity);

        if (await context.SaveChangesAsync() <= 0)
            return RoutesErrorBuilder.RouteUpdateException();

        return VoidResult.Instance;
    }

    private async Task<bool> EntityExists(int id)
    {
        return await context.Routes
            .AsNoTracking()
            .AnyAsync(c => c.Id == id);
    }
}
