using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Application.Drivers.Commands.CreateDrivers;

public class CreateDriversCommand : IRequest<Result<VoidResult, Error>>
{
    public CreateDriversRequestDto Request { get; set; } = default!;
}
