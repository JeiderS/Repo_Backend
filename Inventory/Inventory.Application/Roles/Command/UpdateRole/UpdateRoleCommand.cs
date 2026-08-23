using Inventory.Application.Roles.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Roles.Command.UpdateRole;

public class UpdateRoleCommand : IRequest<Result<RoleDto, Error>>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}
