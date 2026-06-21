
using MediatR;
using Inventory.Application.Routes.Dto;
using Inventory.Domain.Common.Pagination;
using System.Collections.Generic;

namespace Inventory.Application.Routes.Query;

public record GetAllRoutesQuery(PaginationParams PaginationParams) : IRequest<IEnumerable<RoutesDto>>;


