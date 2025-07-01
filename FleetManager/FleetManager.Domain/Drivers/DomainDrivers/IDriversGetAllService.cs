

using FleetManager.Domain.Drivers.Entity;
using FleetManager.Domain.Common.Pagination;

namespace FleetManager.Domain.Drivers.DomainDrivers;

public interface IDriversGetAllService
{
    Task<IEnumerable<DriversEntity>> GetAllAsync(PaginationParams PaginationParams);
}


