using Inventory.Domain.Common.Pagination;
using Inventory.Domain.ScheduleView.Entity;

namespace Inventory.Domain.ScheduleView.DomainScheduleView
{
    public interface IScheduleViewGetAllService
    {
        Task<IEnumerable<ScheduleViewEntity>> GetAllAsync(PaginationParams paginationParams);
    }
}
