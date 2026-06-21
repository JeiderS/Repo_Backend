using Inventory.Domain.Common.Pagination;
using Inventory.Domain.Routes.DomainRoutes;
using Inventory.Domain.Routes.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Routes.DomainService.Impl
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
