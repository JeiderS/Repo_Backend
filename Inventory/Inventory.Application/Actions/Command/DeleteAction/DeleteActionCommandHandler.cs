using Inventory.Application.Actions.Errors;
using Inventory.Domain.Actions.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Actions.Command.DeleteAction;

public class DeleteActionCommandHandler(
    IActionRepository actionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteActionCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(DeleteActionCommand request, CancellationToken cancellationToken)
    {
        var action = await actionRepository.GetByIdAsync(request.Id);
        if (action is null)
            return ActionErrorBuilder.ActionNotFound();

        // No borrar silenciosamente una acción en uso: si está asignada a
        // algún módulo, se rechaza (y por FK, tampoco puede estar concedida
        // a ningún rol sin esa asignación).
        if (await actionRepository.IsAssignedToAnyModuleAsync(action.Id))
            return ActionErrorBuilder.InUseByModules();

        await actionRepository.RemoveAsync(action);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return VoidResult.Instance;
    }
}
