

using Inventory.Domain.Common.Pagination;
using Inventory.Domain.Routes.Entity;


namespace Inventory.Domain.Routes.DomainRoutes;

public interface IRoutesGetAllService
{
    Task<IEnumerable<RoutesEntity>> GetAllAsync(PaginationParams PaginationParams);
}

