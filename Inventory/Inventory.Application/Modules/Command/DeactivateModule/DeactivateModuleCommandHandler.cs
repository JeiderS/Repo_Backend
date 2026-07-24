using Inventory.Application.Modules.Dto;
using Inventory.Application.Modules.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Modules.DomainModules;
using MediatR;

namespace Inventory.Application.Modules.Command.DeactivateModule;

public class DeactivateModuleCommandHandler(
    IModuleGetByIdService moduleGetByIdService,
    IUnitOfWork unitOfWork) : IRequestHandler<DeactivateModuleCommand, Result<ModuleDto, Error>>
{
    public async Task<Result<ModuleDto, Error>> Handle(DeactivateModuleCommand request, CancellationToken cancellationToken)
    {
        var module = await moduleGetByIdService.GetByIdAsync(request.Id);
        if (module is null)
            return ModuleErrorBuilder.ModuleNotFound();

        if (!module.IsActive)
            return ModuleDto.FromEntity(module);

        module.IsActive = false;

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (savedRows <= 0)
            return ModuleErrorBuilder.UpdateException();

        return ModuleDto.FromEntity(module);
    }
}
