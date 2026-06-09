using MediatR;
using FleetManager.Application.Routes.Dto;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Application.Routes.Query.GetRoutesById;

public record GetRoutesByIdQuery(int Id) : IRequest<Result<RoutesDto, Error>>;
