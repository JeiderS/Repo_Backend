using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Vehicles.Entity;

namespace Inventory.Domain.Vehicles.DomainVehicles;

public interface IVehiclesUpdateService
{
    Task<Result<VoidResult, Error>> UpdateAsync(VehiclesEntity vehiclesEntity);
}
