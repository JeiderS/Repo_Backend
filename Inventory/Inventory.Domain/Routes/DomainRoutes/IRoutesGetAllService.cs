

using FleetManager.Domain.Common.Pagination;
using FleetManager.Domain.Routes.Entity;


namespace FleetManager.Domain.Routes.DomainRoutes;

public interface IRoutesGetAllService
{
    Task<IEnumerable<RoutesEntity>> GetAllAsync(PaginationParams PaginationParams);
}

