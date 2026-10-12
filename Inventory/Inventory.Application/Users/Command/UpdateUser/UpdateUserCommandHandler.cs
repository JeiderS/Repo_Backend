using Inventory.Application.Common.Interfaces;
using Inventory.Application.Users.Dto;
using Inventory.Application.Users.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Roles.Repositories;
using Inventory.Domain.Roles.Entity;
using Inventory.Domain.Users.Repositories;
using MediatR;

namespace Inventory.Application.Users.Command.UpdateUser;

public class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IPermissionCache permissionCache,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserCommand, Result<UserDto, Error>>
{
    public async Task<Result<UserDto, Error>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
        if (user is null)
            return UserErrorBuilder.UserNotFound();

        if (await userRepository.EmailExistsAsync(request.Email, excludeUserId: request.Id))
            return UserErrorBuilder.EmailAlreadyExists();

        RoleEntity? role = null;
        if (request.RoleId is not null)
        {
            role = await roleRepository.GetByIdAsync(request.RoleId.Value);
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

        // No usamos las filas afectadas como señal de exito/fracaso: si los
        // valores enviados son identicos a los ya guardados, EF Core no
        // detecta cambios y SaveChangesAsync devuelve 0 filas legitimamente
        // (edicion sin cambios), no un fallo. Una excepcion real de EF/DB
        // sigue propagandose y siendo manejada por el pipeline de errores.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Purga el userId->roleId cacheado en PermissionCache cuando el RoleId
        // cambia (design.md D3): de lo contrario el usuario seguiría
        // autorizando contra el rol anterior hasta que el TTL de 60s expire.
        if (roleChanged)
            permissionCache.InvalidateUser(user.Id);

        return UserDto.FromEntity(user);
    }
}
