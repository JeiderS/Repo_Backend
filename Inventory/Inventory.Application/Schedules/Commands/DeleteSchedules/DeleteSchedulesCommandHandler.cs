using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Schedules.DomainSchedules;

namespace Inventory.Application.Schedules.Commands.DeleteSchedules
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
