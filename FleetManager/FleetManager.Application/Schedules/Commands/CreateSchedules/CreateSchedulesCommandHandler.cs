using AutoMapper;
using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Schedules.DomainSchedules;
using FleetManager.Domain.Schedules.Entity;

namespace FleetManager.Application.Schedules.Commands.CreateSchedules
{
    public class CreateSchedulesCommandHandler(
        ISchedulesCreateService schedulesCreateService,
        IMapper mapper) : IRequestHandler<CreateSchedulesCommand, Result<VoidResult, Error>>
    {
        public async Task<Result<VoidResult, Error>> Handle(CreateSchedulesCommand request, CancellationToken cancellationToken)
        {
            var schedulesEntity = mapper.Map<SchedulesEntity>(request.Request);
            var result = await schedulesCreateService.CreateAsync(schedulesEntity);
            if (!result.IsSuccess)
                return result.Error!;

            return result.Value!;
        }
    }
}
