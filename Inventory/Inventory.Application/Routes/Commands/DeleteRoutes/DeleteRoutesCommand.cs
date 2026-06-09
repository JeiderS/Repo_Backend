using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Common.Results;
using MediatR;

namespace FleetManager.Application.Routes.Commands.DeleteRoutes;

public record DeleteRoutesCommand(int Id) : IRequest<Result<VoidResult, Error>>;
