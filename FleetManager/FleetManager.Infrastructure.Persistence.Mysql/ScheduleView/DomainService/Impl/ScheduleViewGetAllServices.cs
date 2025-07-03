using FleetManager.Domain.Common.Pagination;
using FleetManager.Domain.ScheduleView.Entity;
using FleetManager.Domain.ScheduleView.DomainScheduleView;
using Microsoft.EntityFrameworkCore;
using FleetManager.Infrastructure.Persistence.Mysql.Context;

namespace FleetManager.Infrastructure.Persistence.Mysql.ScheduleView.DomainService.Impl
{
    public class ScheduleViewGetAllService(DataBaseContext context) : IScheduleViewGetAllService
    {
        public async Task<IEnumerable<ScheduleViewEntity>> GetAllAsync(PaginationParams paginationParams)
        {
            return await context.ScheduleViews
                .OrderBy(x => x.Day) 
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();
        }
    }
}
