using Microsoft.EntityFrameworkCore;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Inventory.Domain.Routes.DomainRoutes;
using Inventory.Domain.Routes.Entity;
using Inventory.Application.Routes.Errors;

namespace Inventory.Infrastructure.Persistence.Mysql.Routes.DomainService.Impl;

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
