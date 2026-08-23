using Inventory.Application.Roles.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Roles.Command.CreateRole;

public class CreateRoleCommand : IRequest<Result<RoleDto, Error>>
{
    public string Name { get; set; } = default!;
}
