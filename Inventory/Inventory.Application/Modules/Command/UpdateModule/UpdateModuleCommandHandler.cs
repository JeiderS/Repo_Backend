using Inventory.Application.Modules.Dto;
using Inventory.Application.Modules.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Modules.DomainModules;
using MediatR;

namespace Inventory.Application.Modules.Command.UpdateModule;

public class UpdateModuleCommandHandler(
    IModuleGetByIdService moduleGetByIdService,
    IModuleUpdateService moduleUpdateService,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateModuleCommand, Result<ModuleDto, Error>>
{
    public async Task<Result<ModuleDto, Error>> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
    {
        var module = await moduleGetByIdService.GetByIdAsync(request.Id);
        if (module is null)
            return ModuleErrorBuilder.ModuleNotFound();

        if (await moduleUpdateService.NameExistsForOtherAsync(request.Name, request.Id))
            return ModuleErrorBuilder.NameAlreadyExists();

        if (request.ParentId is not null)
        {
            if (request.ParentId == request.Id)
                return ModuleErrorBuilder.InvalidParent();

            if (await moduleGetByIdService.GetByIdAsync(request.ParentId.Value) is null)
                return ModuleErrorBuilder.ParentNotFound();
        }

        module.Name = request.Name;
        module.Icon = request.Icon;
        module.Route = request.Route;
        module.ParentId = request.ParentId;
        module.SortOrder = request.SortOrder;

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (savedRows <= 0)
            return ModuleErrorBuilder.UpdateException();

        return ModuleDto.FromEntity(module);
    }
}
