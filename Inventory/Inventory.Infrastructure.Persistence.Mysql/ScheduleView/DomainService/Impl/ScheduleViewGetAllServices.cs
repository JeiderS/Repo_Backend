using Inventory.Domain.Common.Pagination;
using Inventory.Domain.ScheduleView.Entity;
using Inventory.Domain.ScheduleView.DomainScheduleView;
using Microsoft.EntityFrameworkCore;
using Inventory.Infrastructure.Persistence.Mysql.Context;

namespace Inventory.Infrastructure.Persistence.Mysql.ScheduleView.DomainService.Impl
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
