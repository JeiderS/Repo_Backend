using FleetManager.Domain.Common.Pagination;
using FleetManager.Domain.Schedules.Entity;

namespace FleetManager.Domain.Schedules.DomainSchedules
{
    public interface ISchedulesGetAllService
    {
        Task<IEnumerable<SchedulesEntity>> GetAllAsync(PaginationParams paginationParams);
    }
}
