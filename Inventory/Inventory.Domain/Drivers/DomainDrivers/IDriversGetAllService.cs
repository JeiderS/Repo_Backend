

using Inventory.Domain.Drivers.Entity;
using Inventory.Domain.Common.Pagination;

namespace Inventory.Domain.Drivers.DomainDrivers;

public interface IDriversGetAllService
{
    Task<IEnumerable<DriversEntity>> GetAllAsync(PaginationParams PaginationParams);
}


