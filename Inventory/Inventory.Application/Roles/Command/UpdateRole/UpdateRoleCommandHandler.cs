using Inventory.Application.Roles.Dto;
using Inventory.Application.Roles.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Roles.DomainRoles;
using MediatR;

namespace Inventory.Application.Roles.Command.UpdateRole;

public class UpdateRoleCommandHandler(
    IRoleGetByIdService roleGetByIdService,
    IRoleUpdateService roleUpdateService,
    IRoleActionAssignService roleActionAssignService,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateRoleCommand, Result<RoleDto, Error>>
{
    public async Task<Result<RoleDto, Error>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleGetByIdService.GetByIdAsync(request.Id);
        if (role is null)
            return RoleErrorBuilder.RoleNotFound();

        if (await roleUpdateService.NameExistsForOtherAsync(request.Name, request.Id))
            return RoleErrorBuilder.NameAlreadyExists();

        role.Name = request.Name;

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (savedRows <= 0)
            return RoleErrorBuilder.UpdateException();

        var actionIds = await roleActionAssignService.GetActionIdsAsync(role.Id);
        return RoleDto.FromEntity(role, actionIds);
    }
}
