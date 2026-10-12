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
    public async Task<Result<ActionDto, Error>> Handle(CreateActionCommand request, CancellationToken cancellationToken)
    {
        var module = await moduleRepository.GetByIdAsync(request.ModuleId);
        if (module is null)
            return ActionErrorBuilder.ModuleNotFound();

        // La regla "el código es {Módulo}{Nombre} en PascalCase" vive en la entidad.
        var created = ActionEntity.Create(module, request.Name);
        if (!created.IsSuccess)
            return created.Error!;

        var action = created.Value!;

        // "El código no se repite" necesita ver todas las acciones: va aquí.
        if (await actionRepository.CodeExistsAsync(action.Code))
            return ActionErrorBuilder.CodeAlreadyExists();

        await actionRepository.AddAsync(action);

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (savedRows <= 0)
            return ActionErrorBuilder.CreationException();

        return ActionDto.FromEntity(action);
    }
}
