using Inventory.Domain.Common.Pagination;
using Inventory.Domain.Drivers.Entity;
using Inventory.Domain.Drivers.DomainDrivers;
using Microsoft.EntityFrameworkCore;
using Inventory.Infrastructure.Persistence.Mysql.Context;

namespace Inventory.Infrastructure.Persistence.Mysql.Drivers.DomainService.Impl
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
