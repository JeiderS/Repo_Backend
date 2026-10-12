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
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateModuleCommand, Result<ModuleDto, Error>>
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

        return ModuleDto.FromEntity(module);
    }
}
