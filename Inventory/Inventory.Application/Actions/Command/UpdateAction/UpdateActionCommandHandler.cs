using Inventory.Application.Actions.Dto;
using Inventory.Application.Actions.Errors;
using Inventory.Domain.Actions.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Actions.Command.UpdateAction;

public class UpdateActionCommandHandler(
    IActionRepository actionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateActionCommand, Result<ActionDto, Error>>
{
    public async Task<Result<ActionDto, Error>> Handle(UpdateActionCommand request, CancellationToken cancellationToken)
    {
        var action = await actionRepository.GetByIdAsync(request.Id);
        if (action is null)
            return ActionErrorBuilder.ActionNotFound();

        var code = request.Code.Trim();
        var name = request.Name.Trim();

        if (await actionRepository.CodeExistsAsync(code, excludeActionId: request.Id))
            return ActionErrorBuilder.CodeAlreadyExists();

        action.Update(code, name);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ActionDto.FromEntity(action);
    }
}
