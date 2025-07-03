using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Schedules.Entity;

namespace FleetManager.Domain.Schedules.DomainSchedules
{
    public interface ISchedulesUpdateService
    {
        Task<Result<VoidResult, Error>> UpdateAsync(SchedulesEntity schedulesEntity);
    }
}
