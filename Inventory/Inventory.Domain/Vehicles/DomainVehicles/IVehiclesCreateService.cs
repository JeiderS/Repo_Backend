using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Vehicles.Entity;

namespace Inventory.Domain.Vehicles.DomainVehicles;

public interface IVehiclesCreateService
{
    Task<Result<VoidResult, Error>> CreateAsync(VehiclesEntity vehiclesEntity);
}
