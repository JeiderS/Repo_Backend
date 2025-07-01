

using Fleet.Domain.Common.Pagination;
using Fleet.Domain.Vehicles.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Fleet.Domain.Vehicles.DomainVehicles
{
    public interface IVehiclesGetAllService
    {
        Task<IEnumerable<VehiclesEntity>> GetAllAsync(PaginationParams PaginationParams);
    }
}


