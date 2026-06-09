using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Application.Schedules.Commands.CreateSchedules
{
    public class CreateSchedulesCommand : IRequest<Result<VoidResult, Error>>
    {
        public CreateSchedulesRequestDto Request { get; set; } = default!;
    }
}
