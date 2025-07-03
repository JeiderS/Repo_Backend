using FleetManager.Application.Schedules.Errors;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Schedules.DomainSchedules;
using FleetManager.Domain.Schedules.Entity;
using FleetManager.Infrastructure.Persistence.Mysql.Context;

namespace FleetManager.Infrastructure.Persistence.Mysql.Schedules.DomainService.Impl
{
    public class SchedulesCreateService(DataBaseContext context) : ISchedulesCreateService
    {
        public async Task<Result<VoidResult, Error>> CreateAsync(SchedulesEntity schedulesEntity)
        {
            await context.Schedules.AddAsync(schedulesEntity);

            var result = await context.SaveChangesAsync() > 0;
            if (!result)
                return SchedulesErrorBuilder.ScheduleCreationException();
            return VoidResult.Instance;
        }
    }
}
