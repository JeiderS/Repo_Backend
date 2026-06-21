using Inventory.Application.Schedules.Errors;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Schedules.DomainSchedules;
using Inventory.Domain.Schedules.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;

namespace Inventory.Infrastructure.Persistence.Mysql.Schedules.DomainService.Impl
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
