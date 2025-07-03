using FleetManager.Domain.Common.Pagination;
using FleetManager.Domain.Schedules.Entity;
using FleetManager.Domain.Schedules.DomainSchedules;
using Microsoft.EntityFrameworkCore;
using FleetManager.Infrastructure.Persistence.Mysql.Context;

namespace FleetManager.Infrastructure.Persistence.Mysql.Schedules.DomainService.Impl
{
    public class SchedulesGetAllService(DataBaseContext context) : ISchedulesGetAllService
    {
        public async Task<IEnumerable<SchedulesEntity>> GetAllAsync(PaginationParams paginationParams)
        {
            return await context.Schedules
                .OrderBy(x => x.Id)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();
        }
    }
}
