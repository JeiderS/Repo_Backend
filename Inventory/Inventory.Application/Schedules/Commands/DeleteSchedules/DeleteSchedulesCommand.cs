using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Common.Results;
using MediatR;

namespace Inventory.Application.Schedules.Commands.DeleteSchedules;

public record DeleteSchedulesCommand(int Id) : IRequest<Result<VoidResult, Error>>;
