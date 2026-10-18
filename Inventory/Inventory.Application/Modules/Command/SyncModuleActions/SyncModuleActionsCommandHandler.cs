using Inventory.Application.Modules.Dto;
using Inventory.Application.Modules.Errors;
using Inventory.Domain.Actions.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Modules.Repositories;
using MediatR;

namespace Inventory.Application.Modules.Command.SyncModuleActions;

public class SyncModuleActionsCommandHandler(
    IModuleRepository moduleRepository,
    IActionRepository actionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SyncModuleActionsCommand, Result<ModuleDto, Error>>
{
    public async Task<Result<ModuleDto, Error>> Handle(SyncModuleActionsCommand request, CancellationToken cancellationToken)
    {
        var module = await moduleRepository.GetByIdAsync(request.ModuleId);
        if (module is null)
            return ModuleErrorBuilder.ModuleNotFound();

        var requestedIds = request.ActionIds.Distinct().ToList();

        // Rechaza si algún id no existe en el catálogo de acciones.
        var existingIds = await actionRepository.GetExistingIdsAsync(requestedIds);
        if (existingIds.Count != requestedIds.Count)
            return ModuleErrorBuilder.ActionNotFound();

        var currentIds = await moduleRepository.GetActionIdsAsync(module.Id);

        var toAdd = requestedIds.Except(currentIds).ToList();
        var toRemove = currentIds.Except(requestedIds).ToList();

        // No borrar silenciosamente permisos en uso: si alguna de las
        // relaciones a quitar está referenciada por RoleActions, se rechaza
        // toda la operación (atómico: nada se aplica parcialmente).
        if (toRemove.Count > 0)
        {
            var inUse = await moduleRepository.GetActionIdsInUseByRolesAsync(module.Id, toRemove);
            if (inUse.Count > 0)
                return ModuleErrorBuilder.ActionsInUseByRoles();
        }

        if (toAdd.Count > 0)
            await moduleRepository.AddModuleActionsAsync(module.Id, toAdd);

        if (toRemove.Count > 0)
            await moduleRepository.RemoveModuleActionsAsync(module.Id, toRemove);

        // Un solo SaveChangesAsync: es el límite transaccional de facto en
        // este proyecto (ver RoleRepository.ReplaceActionsAsync).
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ModuleDto.FromEntity(module, requestedIds);
    }
}
