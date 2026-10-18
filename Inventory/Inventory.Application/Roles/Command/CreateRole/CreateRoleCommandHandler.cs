using Inventory.Application.Roles.Dto;
using Inventory.Application.Roles.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Roles.Entity;
using Inventory.Domain.Roles.Repositories;
using MediatR;

namespace Inventory.Application.Roles.Command.CreateRole;

public class CreateRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateRoleCommand, Result<RoleDto, Error>>
{
    public async Task<Result<RoleDto, Error>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (await roleRepository.NameExistsAsync(request.Name))
            return RoleErrorBuilder.NameAlreadyExists();

        // RoleEntity.Create garantiza que el rol NO es System Admin.
        var role = RoleEntity.Create(request.Name);

        await roleRepository.AddAsync(role);

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (savedRows <= 0)
            return RoleErrorBuilder.CreationException();

        return RoleDto.FromEntity(role, Array.Empty<RoleModuleActionGrant>());
    }
}
