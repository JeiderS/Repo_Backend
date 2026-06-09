using FleetManager.Application.Routes.Errors;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Routes.DomainRoutes;
using FleetManager.Infrastructure.Persistence.Mysql.Context;

namespace FleetManager.Infrastructure.Persistence.Mysql.Routes.DomainService.Impl;

public class RoutesDeleteService(DataBaseContext context) : IRoutesDeleteService
{
    public async Task<Result<VoidResult, Error>> DeleteAsync(int idRoute)
    {
        var entity = await context.Routes.FindAsync(idRoute);
        if (entity == null)
            return RoutesErrorBuilder.RouteNotFound(idRoute);

        context.Routes.Remove(entity);

        var result = await context.SaveChangesAsync() > 0;
        if (!result)
            return RoutesErrorBuilder.RouteDeleteException();

        return VoidResult.Instance;
    }
}
