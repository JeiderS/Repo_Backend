using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Routes.Commands.CreateRoutes;

public class CreateRoutesCommand : IRequest<Result<VoidResult, Error>>
{
    public CreateRoutesRequestDto Request { get; set; } = default!;
}
