using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Drivers.Entity;

namespace Inventory.Domain.Drivers.DomainDrivers;

public interface IDriversGetByIdService
{
    Task<Result<DriversEntity, Error>> GetByIdAsync(int id);
}