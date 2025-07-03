using MediatR;
using FleetManager.Application.Schedules.Dto;
using FleetManager.Domain.Common.Pagination;
using System.Collections.Generic;

namespace FleetManager.Application.Schedules.Query;

public record GetAllSchedulesQuery(PaginationParams PaginationParams) : IRequest<IEnumerable<SchedulesDto>>;
