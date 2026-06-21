using MediatR;
using Inventory.Application.Schedules.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Schedules.Query.GetSchedulesById;

public record GetSchedulesByIdQuery(int Id) : IRequest<Result<SchedulesDto, Error>>;
