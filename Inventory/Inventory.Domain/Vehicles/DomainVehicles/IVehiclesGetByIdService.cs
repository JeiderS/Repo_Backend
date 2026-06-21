using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Vehicles.Entity;

namespace Inventory.Domain.Vehicles.DomainVehicles;

public interface IVehiclesGetByIdService
{
    Task<Result<VehiclesEntity, Error>> GetByIdAsync(int id);
}
