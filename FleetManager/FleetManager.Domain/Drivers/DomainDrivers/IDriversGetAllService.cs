

using Fleet.Domain.Drivers.Entity;
using Fleet.Domain.Common.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fleet.Domain.Drivers.DomainDrivers
{
    public interface IDriversGetAllService
    {
        Task<IEnumerable<DriversEntity>> GetAllAsync(PaginationParams PaginationParams);
    }
}


