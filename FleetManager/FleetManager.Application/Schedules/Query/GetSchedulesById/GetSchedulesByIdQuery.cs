using MediatR;
using FleetManager.Application.Schedules.Dto;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Application.Schedules.Query.GetSchedulesById;

public record GetSchedulesByIdQuery(int Id) : IRequest<Result<SchedulesDto, Error>>;
