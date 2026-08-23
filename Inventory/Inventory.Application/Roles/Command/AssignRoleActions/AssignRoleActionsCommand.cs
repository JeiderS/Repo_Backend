using Inventory.Application.Roles.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Roles.Command.AssignRoleActions;

public class AssignRoleActionsCommand : IRequest<Result<RoleDto, Error>>
{
    public int RoleId { get; set; }
    public IReadOnlyList<int> ActionIds { get; set; } = Array.Empty<int>();
}
