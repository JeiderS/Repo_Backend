using Inventory.Application.Schedules.Errors;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Schedules.DomainSchedules;
using Inventory.Infrastructure.Persistence.Mysql.Context;

namespace Inventory.Infrastructure.Persistence.Mysql.Schedules.DomainService.Impl
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
