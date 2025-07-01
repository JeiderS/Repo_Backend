using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Application.Routes.Commands.CreateRoutes;

public class CreateRoutesCommand : IRequest<Result<VoidResult, Error>>
{
    public CreateRoutesRequestDto Request { get; set; } = default!;
}
