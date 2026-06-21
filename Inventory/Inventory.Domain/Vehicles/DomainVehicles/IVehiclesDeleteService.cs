using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Domain.Vehicles.DomainVehicles
{
    public interface IVehiclesDeleteService
    {
        Task<Result<VoidResult, Error>> DeleteAsync(int id);
    }
}
