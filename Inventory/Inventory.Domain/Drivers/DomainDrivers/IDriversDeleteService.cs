
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Domain.Drivers.DomainDrivers
{
    public interface IDriversDeleteService
    {
        Task<Result<VoidResult, Error>> DeleteAsync(int id);
    }
}
