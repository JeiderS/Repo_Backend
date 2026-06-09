using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Vehicles.Entity;

namespace FleetManager.Domain.Vehicles.DomainVehicles;

public interface IVehiclesUpdateService
{
    Task<Result<VoidResult, Error>> UpdateAsync(VehiclesEntity vehiclesEntity);
}
