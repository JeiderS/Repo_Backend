using FleetManager.Domain.Common.Pagination;
using FleetManager.Domain.Drivers.Entity;
using FleetManager.Domain.Drivers.DomainDrivers;
using Microsoft.EntityFrameworkCore;
using FleetManager.Infrastructure.Persistence.Mysql.Context;

namespace FleetManager.Infrastructure.Persistence.Mysql.Drivers.DomainService.Impl
{
    public class DriversGetAllServices(DataBaseContext context) : IDriversGetAllService
    {
        public async Task<IEnumerable<DriversEntity>> GetAllAsync(PaginationParams paginationParams)
        {
            return await context.Drivers
                .OrderBy(x => x.Id)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();
        }
    }
}
