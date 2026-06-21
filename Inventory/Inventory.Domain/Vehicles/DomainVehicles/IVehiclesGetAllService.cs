

using Inventory.Domain.Common.Pagination;
using Inventory.Domain.Vehicles.Entity;


namespace Inventory.Domain.Vehicles.DomainVehicles;

public interface IVehiclesGetAllService
{
    Task<IEnumerable<VehiclesEntity>> GetAllAsync(PaginationParams PaginationParams);
}


