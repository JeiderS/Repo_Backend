using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Drivers.Entity;

namespace FleetManager.Domain.Drivers.DomainDrivers;

public interface IDriversUpdateService
{
    Task<Result<VoidResult, Error>> UpdateAsync(DriversEntity DriversEntity);
}