using Inventory.Application.Actions.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Actions.Command.UpdateAction;

public class UpdateActionCommand : IRequest<Result<ActionDto, Error>>
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
}
