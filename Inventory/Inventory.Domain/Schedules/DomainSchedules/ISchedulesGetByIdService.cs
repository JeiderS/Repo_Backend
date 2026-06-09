using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Schedules.Entity;

namespace FleetManager.Domain.Schedules.DomainSchedules
{
    public interface ISchedulesGetByIdService
    {
        Task<Result<SchedulesEntity, Error>> GetByIdAsync(int id);
    }
}
