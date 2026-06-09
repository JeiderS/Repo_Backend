using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Routes.Entity;

namespace FleetManager.Domain.Routes.DomainRoutes;

public interface IRoutesUpdateService
{
    Task<Result<VoidResult, Error>> UpdateAsync(RoutesEntity RoutesEntity);
}