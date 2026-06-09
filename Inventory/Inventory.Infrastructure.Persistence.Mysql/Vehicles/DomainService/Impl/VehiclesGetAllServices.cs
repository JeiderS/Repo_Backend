
using FleetManager.Domain.Common.Pagination;
using FleetManager.Domain.Vehicles.DomainVehicles;
using FleetManager.Domain.Vehicles.Entity;
using FleetManager.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace FleetManager.Infrastructure.Persistence.Mysql.Vehicles.DomainService.Impl
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
