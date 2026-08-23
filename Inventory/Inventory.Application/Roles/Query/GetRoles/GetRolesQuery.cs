using Inventory.Application.Roles.Dto;
using MediatR;

namespace Inventory.Application.Roles.Query.GetRoles;

public record GetRolesQuery : IRequest<IReadOnlyList<RoleDto>>;
