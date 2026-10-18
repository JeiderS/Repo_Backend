using Inventory.Application.Modules.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Modules.Command.SyncModuleActions;

public class SyncModuleActionsCommand : IRequest<Result<ModuleDto, Error>>
{
    public int ModuleId { get; set; }
    public IReadOnlyList<int> ActionIds { get; set; } = Array.Empty<int>();
}
