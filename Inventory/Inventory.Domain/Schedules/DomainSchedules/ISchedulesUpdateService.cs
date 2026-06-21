using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Schedules.Entity;

namespace Inventory.Domain.Schedules.DomainSchedules
{
    public interface ISchedulesUpdateService
    {
        Task<Result<VoidResult, Error>> UpdateAsync(SchedulesEntity schedulesEntity);
    }
}
