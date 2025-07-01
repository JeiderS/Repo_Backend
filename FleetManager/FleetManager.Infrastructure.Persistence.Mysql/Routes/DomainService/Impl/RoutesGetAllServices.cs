using FleetManager.Domain.Common.Pagination;
using FleetManager.Domain.Routes.DomainRoutes;
using FleetManager.Domain.Routes.Entity;
using FleetManager.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace FleetManager.Infrastructure.Persistence.Mysql.Routes.DomainService.Impl
{
    public class RoutesGetAllServices(DataBaseContext context) : IRoutesGetAllService
    {
        public async Task<IEnumerable<RoutesEntity>> GetAllAsync(PaginationParams paginationParams)
        {
            return await context.Routes
                .OrderBy(x => x.Id)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();
        }
    }
}
