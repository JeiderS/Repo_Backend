using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Drivers.Commands.CreateDrivers;

public class CreateDriversCommand : IRequest<Result<VoidResult, Error>>
{
    public CreateDriversRequestDto Request { get; set; } = default!;
}
