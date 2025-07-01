

using Fleet.Domain.Common.Pagination;
using Fleet.Domain.Routes.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Fleet.Domain.Routes.DomainRoutes
{
    public interface IRoutesGetAllService
    {
        Task<IEnumerable<RoutesEntity>> GetAllAsync(PaginationParams PaginationParams);
    }

}

