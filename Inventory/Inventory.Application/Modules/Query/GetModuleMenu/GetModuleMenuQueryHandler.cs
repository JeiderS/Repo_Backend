using Inventory.Domain.Modules;
using Inventory.Domain.Modules.Models;
using Inventory.Domain.Modules.Repositories;
using Inventory.Domain.Roles.Repositories;
using Inventory.Domain.Users.Repositories;
using MediatR;

namespace Inventory.Application.Modules.Query.GetModuleMenu;

/// <summary>
/// Antes este caso de uso solo llamaba a ModuleMenuService (Infrastructure),
/// que traía los datos Y decidía qué se veía. Ahora:
///   1. los repositorios solo TRAEN datos,
///   2. ModuleMenuBuilder (Domain) DECIDE qué se ve,
///   3. este handler ORQUESTA los dos pasos.
/// </summary>
public class GetModuleMenuQueryHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IModuleRepository moduleRepository)
    : IRequestHandler<GetModuleMenuQuery, IReadOnlyList<ModuleMenuItem>>
{
    public async Task<IReadOnlyList<ModuleMenuItem>> Handle(GetModuleMenuQuery request, CancellationToken cancellationToken)
    {
        var roleId = await userRepository.GetRoleIdAsync(request.UserId);

        // Sin rol no hay acciones concedidas: el menú queda vacío.
        if (roleId is null)
            return Array.Empty<ModuleMenuItem>();

        var activeModules = await moduleRepository.GetActiveAsync();
        var isSystemAdmin = await roleRepository.IsSystemAdminAsync(roleId.Value);
        var grantedModuleIds = await roleRepository.GetModuleIdsWithGrantedActionsAsync(roleId.Value);

        return ModuleMenuBuilder.Build(activeModules, grantedModuleIds, isSystemAdmin);
    }
}
