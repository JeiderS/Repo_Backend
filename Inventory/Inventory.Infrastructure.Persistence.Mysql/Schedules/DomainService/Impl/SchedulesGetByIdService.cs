using Microsoft.EntityFrameworkCore;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Infrastructure.Persistence.Mysql.Context;
using FleetManager.Domain.Schedules.DomainSchedules;
using FleetManager.Domain.Schedules.Entity;
using FleetManager.Application.Schedules.Errors;

namespace FleetManager.Infrastructure.Persistence.Mysql.Schedules.DomainService.Impl
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
