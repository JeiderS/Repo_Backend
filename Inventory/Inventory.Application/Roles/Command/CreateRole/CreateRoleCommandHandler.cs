using Inventory.Application.Roles.Dto;
using Inventory.Application.Roles.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Roles.DomainRoles;
using Inventory.Domain.Roles.Entity;
using MediatR;

namespace Inventory.Application.Roles.Command.CreateRole;

public class CreateRoleCommandHandler(
    IRoleCreateService roleCreateService,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateRoleCommand, Result<RoleDto, Error>>
{
    public async Task<Result<RoleDto, Error>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (await roleCreateService.NameExistsAsync(request.Name))
            return RoleErrorBuilder.NameAlreadyExists();

        var role = new RoleEntity { Name = request.Name };

        await roleCreateService.AddAsync(role);

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (savedRows <= 0)
            return RoleErrorBuilder.CreationException();

        return RoleDto.FromEntity(role, Array.Empty<int>());
    }
}
