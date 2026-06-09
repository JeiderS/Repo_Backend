using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Vehicles.Entity;

namespace FleetManager.Domain.Vehicles.DomainVehicles;

public interface IVehiclesCreateService
{
    Task<Result<VoidResult, Error>> CreateAsync(VehiclesEntity vehiclesEntity);
}
