using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Drivers.Entity;

namespace FleetManager.Domain.Drivers.DomainDrivers;

public interface IDriversGetByIdService
{
    Task<Result<DriversEntity, Error>> GetByIdAsync(int id);
}