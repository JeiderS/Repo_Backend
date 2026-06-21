using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Drivers.Entity;

namespace Inventory.Domain.Drivers.DomainDrivers;

public interface IDriversCreateService
{
    Task<Result<VoidResult, Error>> CreateAsync(DriversEntity DriversEntity);

}