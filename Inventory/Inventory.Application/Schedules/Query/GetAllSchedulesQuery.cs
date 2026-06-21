using MediatR;
using Inventory.Application.Schedules.Dto;
using Inventory.Domain.Common.Pagination;
using System.Collections.Generic;

namespace Inventory.Application.Schedules.Query;

public record GetAllSchedulesQuery(PaginationParams PaginationParams) : IRequest<IEnumerable<SchedulesDto>>;
