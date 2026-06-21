using AutoMapper;
using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Schedules.DomainSchedules;
using Inventory.Domain.Schedules.Entity;

namespace Inventory.Application.Schedules.Commands.CreateSchedules
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
