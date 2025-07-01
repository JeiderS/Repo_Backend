using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Vehicles.Entity;

namespace FleetManager.Domain.Vehicles.DomainVehicles;

public interface IVehiclesGetByIdService
{
    Task<Result<VehiclesEntity, Error>> GetByIdAsync(int id);
}
