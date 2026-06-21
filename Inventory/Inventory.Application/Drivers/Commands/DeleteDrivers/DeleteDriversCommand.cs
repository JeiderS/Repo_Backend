
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Common.Results;
using MediatR;

namespace Inventory.Application.Drivers.Commands.DeleteDrivers;
public record DeleteDriversCommand(int Id) : IRequest<Result<VoidResult, Error>>;