using Inventory.Application.Modules.Dto;
using Inventory.Application.Modules.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Modules.Repositories;
using MediatR;

namespace Inventory.Application.Modules.Command.DeactivateModule;

public class DeactivateModuleCommandHandler(
    IModuleRepository moduleRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeactivateModuleCommand, Result<ModuleDto, Error>>
{
    public async Task<Result<ModuleDto, Error>> Handle(DeactivateModuleCommand request, CancellationToken cancellationToken)
    {
        var module = await moduleRepository.GetByIdAsync(request.Id);
        if (module is null)
            return ModuleErrorBuilder.ModuleNotFound();

        // La entidad decide: si ya estaba inactivo, no hay nada que guardar.
        if (module.Deactivate())
            await unitOfWork.SaveChangesAsync(cancellationToken);

        return ModuleDto.FromEntity(module);
    }
}
