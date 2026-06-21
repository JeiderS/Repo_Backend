using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Schedules.Commands.CreateSchedules
{
    public class CreateSchedulesCommand : IRequest<Result<VoidResult, Error>>
    {
        public CreateSchedulesRequestDto Request { get; set; } = default!;
    }
}
