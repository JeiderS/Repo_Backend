using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Domain.Vehicles.DomainVehicles
{
    public interface IVehiclesDeleteService
    {
        Task<Result<VoidResult, Error>> DeleteAsync(int id);
    }
}
