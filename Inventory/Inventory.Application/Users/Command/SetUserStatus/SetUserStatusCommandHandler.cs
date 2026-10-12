using Inventory.Application.Common.Interfaces;
using Inventory.Application.Users.Dto;
using Inventory.Application.Users.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Users.Repositories;
using MediatR;

namespace Inventory.Application.Users.Command.SetUserStatus;

public class SetUserStatusCommandHandler(
    IUserRepository userRepository,
    IActiveUserCache activeUserCache,
    IPermissionCache permissionCache,
    IUnitOfWork unitOfWork) : IRequestHandler<SetUserStatusCommand, Result<UserDto, Error>>
{
    public async Task<Result<UserDto, Error>> Handle(SetUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
        if (user is null)
            return UserErrorBuilder.UserNotFound();

        if (request.IsActive)
            user.Activate();
        else
            user.Deactivate();

        // 0 filas guardadas NO es un error: desactivar a alguien que ya estaba
        // inactivo no cambia nada. Un fallo real llega como excepción.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalida el cache en la misma unidad de trabajo que cambia IsActive
        // (design.md decision), así ActiveUserValidationMiddleware ve el nuevo
        // estado de inmediato en lugar de esperar el TTL de 60s.
        activeUserCache.Invalidate(user.Id);

        // Purga también el permission cache, en ambas direcciones (design.md D7):
        // evita que una entrada userId->roleId quede obsoleta si el usuario es
        // reactivado más tarde. Spec: action-code-authorization "Permission
        // Cache Purge on Deactivation".
        permissionCache.InvalidateUser(user.Id);

        return UserDto.FromEntity(user);
    }
}
