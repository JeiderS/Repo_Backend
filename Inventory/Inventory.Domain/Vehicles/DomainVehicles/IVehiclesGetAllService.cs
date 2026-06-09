

using FleetManager.Domain.Common.Pagination;
using FleetManager.Domain.Vehicles.Entity;


namespace FleetManager.Domain.Vehicles.DomainVehicles;

public interface IVehiclesGetAllService
{
    Task<IEnumerable<VehiclesEntity>> GetAllAsync(PaginationParams PaginationParams);
}


