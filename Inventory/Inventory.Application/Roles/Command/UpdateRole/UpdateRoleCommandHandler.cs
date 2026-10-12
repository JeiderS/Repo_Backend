using Inventory.Application.Roles.Dto;
using Inventory.Application.Roles.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Roles.Repositories;
using MediatR;

namespace Inventory.Application.Roles.Command.UpdateRole;

public class UpdateRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateRoleCommand, Result<RoleDto, Error>>
{
    public async Task<Result<RoleDto, Error>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id);
        if (role is null)
            return RoleErrorBuilder.RoleNotFound();

        if (await roleRepository.NameExistsAsync(request.Name, excludeRoleId: request.Id))
            return RoleErrorBuilder.NameAlreadyExists();

        role.Rename(request.Name);

        // 0 filas guardadas NO es un error: si los datos no cambiaron, EF no
        // actualiza nada. Un fallo real llega como excepción.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var actionIds = await roleRepository.GetActionIdsAsync(role.Id);
        return RoleDto.FromEntity(role, actionIds);
    }
}
