using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Routes.Entity;

namespace Inventory.Domain.Routes.DomainRoutes;

public interface IRoutesUpdateService
{
    Task<Result<VoidResult, Error>> UpdateAsync(RoutesEntity RoutesEntity);
}