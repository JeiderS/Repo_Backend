
using FleetManager.Domain.Common.Results.Errors;
using FleetManager.Domain.Common.Results;
using MediatR;

namespace FleetManager.Application.Drivers.Commands.DeleteDrivers;
public record DeleteDriversCommand(int Id) : IRequest<Result<VoidResult, Error>>;