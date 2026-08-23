using Inventory.Application.Common.Interfaces;
using Inventory.Application.Users.Dto;
using Inventory.Application.Users.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Roles.DomainRoles;
using Inventory.Domain.Roles.Entity;
using Inventory.Domain.Users.DomainUsers;
using MediatR;

namespace Inventory.Application.Users.Command.UpdateUser;

public class UpdateUserCommandHandler(
    IUserGetByIdService userGetByIdService,
    IUserUpdateService userUpdateService,
    IRoleGetByIdService roleGetByIdService,
    IPermissionCache permissionCache,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserCommand, Result<UserDto, Error>>
{
    public async Task<Result<UserDto, Error>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userGetByIdService.GetByIdAsync(request.Id);
        if (user is null)
            return UserErrorBuilder.UserNotFound();

        if (await userUpdateService.EmailExistsForOtherAsync(request.Email, request.Id))
            return UserErrorBuilder.EmailAlreadyExists();

        RoleEntity? role = null;
        if (request.RoleId is not null)
        {
            role = await roleGetByIdService.GetByIdAsync(request.RoleId.Value);
            if (role is null)
                return UserErrorBuilder.RoleNotFound();
        }

        var roleChanged = user.RoleId != request.RoleId;

        user.Email = request.Email;
        // Asignar un nuevo RoleId reemplaza el anterior: es un FK simple, no una
        // colección (spec: user-administration, "Reassigning a role replaces the
        // previous one"). Pasar RoleId = null también es válido: deja al usuario
        // sin rol.
        user.RoleId = request.RoleId;
        user.Role = role;

        if (user.Profile is not null)
        {
            user.Profile.FirstName = request.FirstName;
            user.Profile.LastName = request.LastName;
            user.Profile.Phone = request.Phone;
            user.Profile.Address = request.Address;
        }

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (savedRows <= 0)
            return UserErrorBuilder.UpdateException();

        // Purga el userId->roleId cacheado en PermissionCache cuando el RoleId
        // cambia (design.md D3): de lo contrario el usuario seguiría
        // autorizando contra el rol anterior hasta que el TTL de 60s expire.
        if (roleChanged)
            permissionCache.InvalidateUser(user.Id);

        return UserDto.FromEntity(user);
    }
}
