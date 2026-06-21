using Microsoft.EntityFrameworkCore;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Inventory.Domain.Schedules.DomainSchedules;
using Inventory.Domain.Schedules.Entity;
using Inventory.Application.Schedules.Errors;

namespace Inventory.Infrastructure.Persistence.Mysql.Schedules.DomainService.Impl
{
    public class SchedulesGetByIdService(DataBaseContext context) : ISchedulesGetByIdService
    {
        public async Task<Result<SchedulesEntity, Error>> GetByIdAsync(int id)
        {
            if (!await EntityExists(id))
                return SchedulesErrorBuilder.ScheduleNotFoundException(id);

            var data = await context.Schedules
                .FirstAsync(c => c.Id == id);

            return data;
        }

        private async Task<bool> EntityExists(int id)
        {
            return await context.Schedules
                .AsNoTracking()
                .AnyAsync(c => c.Id == id);
        }
    }
}
