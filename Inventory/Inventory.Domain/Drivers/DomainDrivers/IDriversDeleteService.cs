
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Domain.Drivers.DomainDrivers
{
    public interface IDriversDeleteService
    {
        Task<Result<VoidResult, Error>> DeleteAsync(int id);
    }
}
