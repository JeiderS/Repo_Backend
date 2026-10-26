using Inventory.Application.Actions.Command.CreateAction;
using Inventory.Application.Modules.Command.SyncModuleActions;
using Inventory.Application.Modules.Dto;
using Inventory.Application.Modules.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Modules.Repositories;
using MediatR;

namespace Inventory.Application.Modules.Command.UpdateModule;

public class UpdateModuleCommandHandler(
    IModuleRepository moduleRepository,
    IUnitOfWork unitOfWork,
    ISender sender) : IRequestHandler<UpdateModuleCommand, Result<ModuleDto, Error>>
{
    public async Task<Result<ModuleDto, Error>> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
    {
        var module = await moduleRepository.GetByIdAsync(request.Id);
        if (module is null)
            return ModuleErrorBuilder.ModuleNotFound();

        // Reglas que necesitan la base de datos: se comprueban aquí.
        if (await moduleRepository.NameExistsAsync(request.Name, excludeModuleId: request.Id))
            return ModuleErrorBuilder.NameAlreadyExists();

        if (request.ParentId is not null && !await moduleRepository.ExistsAsync(request.ParentId.Value))
            return ModuleErrorBuilder.ParentNotFound();

        // Reglas propias del módulo (ej. "no puede ser su propio padre"): las
        // aplica la entidad.
        var update = module.Update(request.Name, request.Icon, request.Route, request.ParentId, request.SortOrder);
        if (!update.IsSuccess)
            return update.Error!;

        // 0 filas guardadas NO es un error: si los datos no cambiaron, EF no
        // actualiza nada. Un fallo real llega como excepción.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var hasNewActions = request.NewActions is { Count: > 0 };

        // ActionIds/NewActions son opcionales: si no vino ninguno, no se
        // tocan las acciones del módulo.
        if (request.ActionIds is not null || hasNewActions)
        {
            var targetActionIds = (request.ActionIds ?? Array.Empty<int>()).ToList();

            // Cada accion nueva se crea aqui dentro, en el mismo request,
            // para que no dependa del rol "AccionesCreate" del endpoint
            // POST /actions: quien puede editar el modulo ya puede crear las
            // acciones que ese modulo necesita.
            foreach (var newAction in request.NewActions ?? Array.Empty<NewModuleActionItem>())
            {
                var createResult = await sender.Send(
                    new CreateActionCommand(newAction.Code, newAction.Name),
                    cancellationToken);

                if (!createResult.IsSuccess)
                    return createResult.Error!;

                targetActionIds.Add(createResult.Value!.Id);
            }

            // El diff de altas/bajas y la proteccion contra quitar acciones
            // en uso por roles viven en SyncModuleActionsCommand: se reusa
            // en vez de duplicarla.
            var syncResult = await sender.Send(
                new SyncModuleActionsCommand { ModuleId = module.Id, ActionIds = targetActionIds },
                cancellationToken);
            return syncResult;
        }

        var currentActionIds = await moduleRepository.GetActionIdsAsync(module.Id);
        return ModuleDto.FromEntity(module, currentActionIds);
    }
}
