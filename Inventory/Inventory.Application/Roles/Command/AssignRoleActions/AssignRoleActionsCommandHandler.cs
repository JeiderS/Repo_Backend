using Inventory.Application.Common.Interfaces;
using Inventory.Application.Roles.Dto;
using Inventory.Application.Roles.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Modules.Repositories;
using Inventory.Domain.Roles.Repositories;
using MediatR;

namespace Inventory.Application.Roles.Command.AssignRoleActions;

public class AssignRoleActionsCommandHandler(
    IRoleRepository roleRepository,
    IModuleRepository moduleRepository,
    IPermissionCache permissionCache,
    IUnitOfWork unitOfWork) : IRequestHandler<AssignRoleActionsCommand, Result<RoleDto, Error>>
{
    public async Task<Result<RoleDto, Error>> Handle(AssignRoleActionsCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.RoleId);
        if (role is null)
            return RoleErrorBuilder.RoleNotFound();

        var requestedGrants = request.Grants
            .Distinct()
            .ToList();

        // Cada par (ModuleId, ActionId) debe existir realmente en
        // ModuleActions — ya no basta con que el ActionId exista en el
        // catálogo global, porque una misma acción puede estar (o no) atada
        // a cada módulo concreto.
        var actionIdsByModule = await moduleRepository.GetActionIdsByModuleAsync();
        var allValid = requestedGrants.All(grant =>
            actionIdsByModule.TryGetValue(grant.ModuleId, out var moduleActionIds)
            && moduleActionIds.Contains(grant.ActionId));

        if (!allValid)
            return RoleErrorBuilder.ActionNotFound();

        // Reemplazo total: no se comprueba SaveChangesAsync > 0, un reemplazo
        // vacío-a-vacío es un no-op válido, no un error. El repositorio
        // (Domain/Infrastructure) recibe tuplas en vez del record
        // RoleModuleActionGrant: ese record vive en Application y Domain no
        // puede referenciar Application.
        await roleRepository.ReplaceActionsAsync(
            role.Id,
            requestedGrants.Select(g => (g.ModuleId, g.ActionId)));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalida el permission cache de este rol para que quede
        // runtime-effective en la próxima request, sujeto al TTL de la cache
        // (spec: role-permission-management, "Role Action Assignment").
        permissionCache.InvalidateRole(role.Id);

        return RoleDto.FromEntity(role, requestedGrants);
    }
}
