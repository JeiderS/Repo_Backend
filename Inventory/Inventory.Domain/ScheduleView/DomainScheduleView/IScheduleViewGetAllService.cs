using FleetManager.Domain.Common.Pagination;
using FleetManager.Domain.ScheduleView.Entity;

namespace FleetManager.Domain.ScheduleView.DomainScheduleView
{
    public interface IScheduleViewGetAllService
    {
        Task<IEnumerable<ScheduleViewEntity>> GetAllAsync(PaginationParams paginationParams);
    }
}
