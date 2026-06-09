using Microsoft.EntityFrameworkCore;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Infrastructure.Persistence.Mysql.Context;
using FleetManager.Domain.Routes.DomainRoutes;
using FleetManager.Domain.Routes.Entity;
using FleetManager.Application.Routes.Errors;

namespace FleetManager.Infrastructure.Persistence.Mysql.Routes.DomainService.Impl;

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
