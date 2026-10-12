using Inventory.Application.Roles.Dto;
using Inventory.Domain.Roles.Repositories;
using MediatR;

namespace Inventory.Application.Roles.Query.GetRoles;

public class GetRolesQueryHandler(IRoleRepository roleRepository)
    : IRequestHandler<GetRolesQuery, IReadOnlyList<RoleDto>>
{
    public async Task<IReadOnlyList<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleRepository.GetAllAsync();

        // Una sola consulta para las acciones de todos los roles (antes era
        // una consulta por rol: el clásico problema "N+1").
        var actionIdsByRole = await roleRepository.GetActionIdsByRoleAsync();

        return roles
            .Select(role => RoleDto.FromEntity(
                role,
                actionIdsByRole.TryGetValue(role.Id, out var actionIds) ? actionIds : Array.Empty<int>()))
            .ToList();
    }
}
