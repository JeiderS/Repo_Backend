using AutoMapper;
using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Schedules.DomainSchedules;
using Inventory.Domain.Schedules.Entity;

namespace Inventory.Application.Schedules.Commands.UpdateSchedules;

public class UpdateSchedulesCommandHandler(ISchedulesUpdateService schedulesUpdateService, IMapper mapper)
    : IRequestHandler<UpdateSchedulesCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(UpdateSchedulesCommand request, CancellationToken cancellationToken)
    {
        var schedulesEntity = mapper.Map<SchedulesEntity>(request);
        var result = await schedulesUpdateService.UpdateAsync(schedulesEntity);

        if (!result.IsSuccess)
            return result.Error!;

        return result.Value!;
    }
}
