using Inventory.Domain.Common.Authorization;
using Inventory.Domain.Roles.Repositories;
using Inventory.Domain.Users.Repositories;
using MediatR;

namespace Inventory.Application.Auth.Queries.GetAuthorizationData;

/// <summary>
/// Foto de roles + permisos por módulo, calculada en vivo contra la BD (no se
/// cachea en el token) para que un cambio de permisos sea efectivo de
/// inmediato. La regla "el sufijo del código define el permiso" vive en
/// <see cref="ModulePermission.FromActionCodes"/>.
/// </summary>
public class GetAuthorizationDataQueryHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository)
    : IRequestHandler<GetAuthorizationDataQuery, UserAuthorizationData>
{
    public async Task<UserAuthorizationData> Handle(GetAuthorizationDataQuery request, CancellationToken cancellationToken)
    {
        var roleId = await userRepository.GetRoleIdAsync(request.UserId);
        if (roleId is null)
            return UserAuthorizationData.Empty;

        var role = await roleRepository.GetByIdAsync(roleId.Value);
        if (role is null)
            return UserAuthorizationData.Empty;

        var grantedActions = await roleRepository.GetGrantedActionsAsync(role.Id);

        var permissions = grantedActions
            .GroupBy(a => a.ModuleName)
            .Select(g => ModulePermission.FromActionCodes(g.Key, g.Select(a => a.ActionCode)))
            .ToList();

        return new UserAuthorizationData(new[] { role.Name }, permissions);
    }
}
