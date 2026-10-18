using Inventory.Application.Actions.Dto;
using Inventory.Application.Actions.Errors;
using Inventory.Domain.Actions.Entity;
using Inventory.Domain.Actions.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Modules.Repositories;
using MediatR;

namespace Inventory.Application.Actions.Command.CreateAction;

public class CreateActionCommandHandler(
    IModuleRepository moduleRepository,
    IActionRepository actionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateActionCommand, Result<ActionDto, Error>>
{
    public async Task<Result<ActionDto, Error>> Handle(
        CreateActionCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.Code.Trim();
        var name = request.Name.Trim();

        if (await actionRepository.CodeExistsAsync(code))
            return ActionErrorBuilder.CodeAlreadyExists();

        var action = ActionEntity.Create(code, name);

        await actionRepository.AddAsync(action);

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (savedRows <= 0)
            return ActionErrorBuilder.CreationException();

        return ActionDto.FromEntity(action);
    }
}
