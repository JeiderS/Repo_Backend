
using MediatR;
using FleetManager.Application.Routes.Dto;
using FleetManager.Domain.Common.Pagination;
using System.Collections.Generic;

namespace FleetManager.Application.Routes.Query;

public record GetAllRoutesQuery(PaginationParams paginationParams) : IRequest<IEnumerable<RoutesDto>>;


