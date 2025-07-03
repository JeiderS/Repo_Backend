using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Common.Results;
using MediatR;

namespace FleetManager.Application.Schedules.Commands.DeleteSchedules;

public record DeleteSchedulesCommand(int Id) : IRequest<Result<VoidResult, Error>>;
