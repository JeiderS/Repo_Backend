using Inventory.Domain.Common.Pagination;
using Inventory.Domain.Schedules.Entity;
using Inventory.Domain.Schedules.DomainSchedules;
using Microsoft.EntityFrameworkCore;
using Inventory.Infrastructure.Persistence.Mysql.Context;

namespace Inventory.Infrastructure.Persistence.Mysql.Schedules.DomainService.Impl
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
