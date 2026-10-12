using Inventory.Application.Common.Interfaces;
using Inventory.Application.Users.Dto;
using Inventory.Application.Users.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Roles.Entity;
using Inventory.Domain.Roles.Repositories;
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

        user.ChangeEmail(request.Email);

        // La entidad sabe que asignar un rol REEMPLAZA el anterior, y nos
        // avisa si cambió.
        var roleChanged = user.AssignRole(role);

        user.Profile?.Update(request.FirstName, request.LastName, request.Phone, request.Address);

        // No usamos las filas afectadas como señal de éxito/fracaso: si los
        // valores enviados son idénticos a los ya guardados, EF Core no
        // detecta cambios y SaveChangesAsync devuelve 0 filas legítimamente
        // (edición sin cambios), no un fallo. Una excepción real de EF/DB
        // sigue propagándose y la maneja el GlobalExceptionHandler.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Purga el userId->roleId cacheado cuando el rol cambia (design.md D3):
        // de lo contrario el usuario seguiría autorizando contra el rol
        // anterior hasta que el TTL de 60s expire.
        if (roleChanged)
            permissionCache.InvalidateUser(user.Id);

        return UserDto.FromEntity(user);
    }
}
