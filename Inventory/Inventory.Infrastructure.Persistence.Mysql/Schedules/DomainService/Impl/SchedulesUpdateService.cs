using Microsoft.EntityFrameworkCore;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Inventory.Domain.Schedules.DomainSchedules;
using Inventory.Domain.Schedules.Entity;
using Inventory.Application.Schedules.Errors;

namespace Inventory.Infrastructure.Persistence.Mysql.Schedules.DomainService.Impl
{
    public class SchedulesUpdateService(DataBaseContext context) : ISchedulesUpdateService
    {
        public async Task<Result<VoidResult, Error>> UpdateAsync(SchedulesEntity schedulesEntity)
        {
            if (!await EntityExists(schedulesEntity.Id))
                return SchedulesErrorBuilder.ScheduleNotFoundException(schedulesEntity.Id);

            context.Schedules.Update(schedulesEntity);

            if (await context.SaveChangesAsync() <= 0)
                return SchedulesErrorBuilder.ScheduleUpdateException();

            return VoidResult.Instance;
        }

        private async Task<bool> EntityExists(int id)
        {
            return await context.Schedules
                .AsNoTracking()
                .AnyAsync(c => c.Id == id);
        }
    }
}
