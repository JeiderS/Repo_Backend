using Inventory.Application.Common.Interfaces;
using Inventory.Application.Roles.Dto;
using Inventory.Application.Roles.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Roles.DomainRoles;
using MediatR;

namespace Inventory.Application.Roles.Command.AssignRoleActions;

public class AssignRoleActionsCommandHandler(
    IRoleGetByIdService roleGetByIdService,
    IRoleActionAssignService roleActionAssignService,
    IPermissionCache permissionCache,
    IUnitOfWork unitOfWork) : IRequestHandler<AssignRoleActionsCommand, Result<RoleDto, Error>>
{
    public async Task<Result<RoleDto, Error>> Handle(AssignRoleActionsCommand request, CancellationToken cancellationToken)
    {
        var role = await roleGetByIdService.GetByIdAsync(request.RoleId);
        if (role is null)
            return RoleErrorBuilder.RoleNotFound();

        var requestedIds = request.ActionIds.Distinct().ToList();

        // Rechaza la asignación si algún id no existe en el catálogo sembrado
        // (spec: role-permission-management, "Assigning a non-existent action is rejected").
        var existingIds = await roleActionAssignService.GetExistingActionIdsAsync(requestedIds);
        if (existingIds.Count != requestedIds.Count)
            return RoleErrorBuilder.ActionNotFound();

        // Reemplazo total: no se comprueba SaveChangesAsync > 0, un reemplazo
        // vacío-a-vacío es un no-op válido, no un error.
        await roleActionAssignService.ReplaceActionsAsync(role.Id, requestedIds);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalida el permission cache de este rol para que quede
        // runtime-effective en la próxima request, sujeto al TTL de la cache
        // (spec: role-permission-management, "Role Action Assignment").
        permissionCache.InvalidateRole(role.Id);

        return RoleDto.FromEntity(role, requestedIds);
    }
}
