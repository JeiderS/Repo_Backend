using Inventory.Application.Actions.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Actions.Command.CreateAction;

public class CreateActionCommand : IRequest<Result<ActionDto, Error>>
{
    public int ModuleId { get; set; }
    public string Name { get; set; } = default!;
}
