
using Inventory.Domain.Common.Pagination;
using Inventory.Domain.Vehicles.DomainVehicles;
using Inventory.Domain.Vehicles.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Vehicles.DomainService.Impl
{
    public class VehiclesGetAllServices(DataBaseContext context) : IVehiclesGetAllService
    {
        public async Task<IEnumerable<VehiclesEntity>> GetAllAsync(PaginationParams paginationParams)
        {
            return await context.Vehicles
                .OrderBy(x => x.Id)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();
        }
    }
}
