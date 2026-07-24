using Inventory.Application.Modules.Dto;
using Inventory.Application.Modules.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Modules.DomainModules;
using Inventory.Domain.Modules.Entity;
using MediatR;

namespace Inventory.Application.Modules.Command.CreateModule;

public class CreateModuleCommandHandler(
    IModuleCreateService moduleCreateService,
    IModuleGetByIdService moduleGetByIdService,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateModuleCommand, Result<ModuleDto, Error>>
{
    public async Task<Result<ModuleDto, Error>> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
    {
        if (await moduleCreateService.NameExistsAsync(request.Name))
            return ModuleErrorBuilder.NameAlreadyExists();

        if (request.ParentId is not null && await moduleGetByIdService.GetByIdAsync(request.ParentId.Value) is null)
            return ModuleErrorBuilder.ParentNotFound();

        var module = new ModuleEntity
        {
            Name = request.Name,
            Icon = request.Icon,
            Route = request.Route,
            ParentId = request.ParentId,
            SortOrder = request.SortOrder,
            IsActive = true
        };

        await moduleCreateService.AddAsync(module);

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (savedRows <= 0)
            return ModuleErrorBuilder.CreationException();

        return ModuleDto.FromEntity(module);
    }
}
