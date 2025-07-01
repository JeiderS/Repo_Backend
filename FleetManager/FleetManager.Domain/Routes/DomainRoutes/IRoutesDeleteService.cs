
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Domain.Routes.DomainRoutes
{
    public interface IRoutesDeleteService
    {
        Task<Result<VoidResult, Error>> DeleteAsync(int id);
    }
}
