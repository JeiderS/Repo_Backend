using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Domain.Schedules.DomainSchedules
{
    public interface ISchedulesDeleteService
    {
        Task<Result<VoidResult, Error>> DeleteAsync(int id);
    }
}
