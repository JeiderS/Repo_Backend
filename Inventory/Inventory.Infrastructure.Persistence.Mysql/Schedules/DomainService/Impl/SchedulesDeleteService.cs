using FleetManager.Application.Schedules.Errors;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Schedules.DomainSchedules;
using FleetManager.Infrastructure.Persistence.Mysql.Context;

namespace FleetManager.Infrastructure.Persistence.Mysql.Schedules.DomainService.Impl
{
    public class SchedulesDeleteService(DataBaseContext context) : ISchedulesDeleteService
    {
        public async Task<Result<VoidResult, Error>> DeleteAsync(int idSchedule)
        {
            var entity = await context.Schedules.FindAsync(idSchedule);
            if (entity == null)
                return SchedulesErrorBuilder.ScheduleNotFound(idSchedule);

            context.Schedules.Remove(entity);

            var result = await context.SaveChangesAsync() > 0;
            if (!result)
                return SchedulesErrorBuilder.ScheduleDeleteException();

            return VoidResult.Instance;
        }
    }
}
