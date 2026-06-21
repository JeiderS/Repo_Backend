using Inventory.Domain.Common.Pagination;
using Inventory.Domain.Schedules.Entity;

namespace Inventory.Domain.Schedules.DomainSchedules
{
    public interface ISchedulesGetAllService
    {
        Task<IEnumerable<SchedulesEntity>> GetAllAsync(PaginationParams paginationParams);
    }
}
