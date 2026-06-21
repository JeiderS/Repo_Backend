using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Domain.Schedules.DomainSchedules
{
    public interface ISchedulesDeleteService
    {
        Task<Result<VoidResult, Error>> DeleteAsync(int id);
    }
}
