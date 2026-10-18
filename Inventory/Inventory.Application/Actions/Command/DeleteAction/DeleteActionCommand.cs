using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Actions.Command.DeleteAction;

public sealed record DeleteActionCommand(int Id) : IRequest<Result<VoidResult, Error>>;
