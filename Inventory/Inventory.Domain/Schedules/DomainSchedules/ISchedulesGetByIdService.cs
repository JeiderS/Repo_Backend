using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Schedules.Entity;

namespace Inventory.Domain.Schedules.DomainSchedules
{
    public interface ISchedulesGetByIdService
    {
        Task<Result<SchedulesEntity, Error>> GetByIdAsync(int id);
    }
}
