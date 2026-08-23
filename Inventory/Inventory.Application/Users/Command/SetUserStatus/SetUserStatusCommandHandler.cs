using Inventory.Application.Common.Interfaces;
using Inventory.Application.Users.Dto;
using Inventory.Application.Users.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Users.DomainUsers;
using MediatR;

namespace Inventory.Application.Users.Command.SetUserStatus;

public class SetUserStatusCommandHandler(
    IUserStatusService userStatusService,
    IActiveUserCache activeUserCache,
    IUnitOfWork unitOfWork) : IRequestHandler<SetUserStatusCommand, Result<UserDto, Error>>
{
    public async Task<Result<UserDto, Error>> Handle(SetUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await userStatusService.GetByIdAsync(request.Id);
        if (user is null)
            return UserErrorBuilder.UserNotFound();

        // Sin auto-protección (spec: role-permission-management, "No Self-Protection
        // on Role Removal" aplica el mismo principio aquí): un admin puede
        // desactivar su propia cuenta, no se bloquea por id.
        user.IsActive = request.IsActive;

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (savedRows <= 0)
            return UserErrorBuilder.UpdateException();

        // Invalida el cache en la misma unidad de trabajo que cambia IsActive
        // (design.md decision), así ActiveUserValidationMiddleware ve el nuevo
        // estado de inmediato en lugar de esperar el TTL de 60s.
        activeUserCache.Invalidate(user.Id);

        return UserDto.FromEntity(user);
    }
}
