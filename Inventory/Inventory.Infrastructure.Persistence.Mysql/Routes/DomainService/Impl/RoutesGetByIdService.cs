using Microsoft.EntityFrameworkCore;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Inventory.Domain.Routes.DomainRoutes;
using Inventory.Domain.Routes.Entity;
using Inventory.Application.Routes.Errors;

namespace Inventory.Infrastructure.Persistence.Mysql.Routes.DomainService.Impl;

public class RoutesGetByIdService(DataBaseContext context) : IRoutesGetByIdService
{
    public async Task<Result<RoutesEntity, Error>> GetByIdAsync(int id)
    {
        if (!await EntityExists(id))
            return RoutesErrorBuilder.RouteNotFoundException(id);

        var data = await context.Routes
            .FirstAsync(c => c.Id == id);

        return data;
    }

    private async Task<bool> EntityExists(int id)
    {
        return await context.Routes
            .AsNoTracking()
            .AnyAsync(c => c.Id == id);
    }
}
