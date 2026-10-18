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

        // Una sola consulta para las concesiones de todos los roles (antes era
        // una consulta por rol: el clásico problema "N+1"). El repositorio
        // devuelve tuplas (ModuleId, ActionId); se mapean a RoleModuleActionGrant
        // aquí en Application.
        var grantsByRole = await roleRepository.GetGrantedModuleActionsByRoleAsync();

        return roles
            .Select(role => RoleDto.FromEntity(
                role,
                grantsByRole.TryGetValue(role.Id, out var grants)
                    ? grants.Select(g => new RoleModuleActionGrant(g.ModuleId, g.ActionId)).ToList()
                    : Array.Empty<RoleModuleActionGrant>()))
            .ToList();
    }
}
