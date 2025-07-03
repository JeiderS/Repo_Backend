using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Schedules.DomainSchedules;

namespace FleetManager.Application.Schedules.Commands.DeleteSchedules
{
    public class DeleteSchedulesCommandHandler(ISchedulesDeleteService schedulesDeleteService)
        : IRequestHandler<DeleteSchedulesCommand, Result<VoidResult, Error>>
    {
        public async Task<Result<VoidResult, Error>> Handle(DeleteSchedulesCommand request, CancellationToken cancellationToken)
        {
            var result = await schedulesDeleteService.DeleteAsync(request.Id);

            if (!result.IsSuccess)
                return result.Error!;

            return result.Value!;
        }
    }
}
