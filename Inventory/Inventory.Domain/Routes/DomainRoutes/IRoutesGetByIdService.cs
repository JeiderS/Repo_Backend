using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Routes.Entity;

namespace FleetManager.Domain.Routes.DomainRoutes;

public interface IRoutesGetByIdService
{
    Task<Result<RoutesEntity, Error>> GetByIdAsync(int id);
}