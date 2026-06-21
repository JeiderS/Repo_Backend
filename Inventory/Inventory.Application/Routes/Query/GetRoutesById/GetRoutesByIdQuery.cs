using MediatR;
using Inventory.Application.Routes.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Routes.Query.GetRoutesById;

public record GetRoutesByIdQuery(int Id) : IRequest<Result<RoutesDto, Error>>;
