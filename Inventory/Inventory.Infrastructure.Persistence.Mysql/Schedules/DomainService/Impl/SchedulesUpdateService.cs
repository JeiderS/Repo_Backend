using Microsoft.EntityFrameworkCore;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Infrastructure.Persistence.Mysql.Context;
using FleetManager.Domain.Schedules.DomainSchedules;
using FleetManager.Domain.Schedules.Entity;
using FleetManager.Application.Schedules.Errors;

namespace FleetManager.Infrastructure.Persistence.Mysql.Schedules.DomainService.Impl
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
