using Inventory.Application.Routes.Errors;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Routes.DomainRoutes;
using Inventory.Infrastructure.Persistence.Mysql.Context;

namespace Inventory.Infrastructure.Persistence.Mysql.Routes.DomainService.Impl;

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
