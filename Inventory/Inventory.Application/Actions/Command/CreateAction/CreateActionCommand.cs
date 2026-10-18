using Inventory.Application.Actions.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Actions.Command.CreateAction;

public sealed record CreateActionCommand(
    string Code,
    string Name
) : IRequest<Result<ActionDto, Error>>;