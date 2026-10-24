using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.RefreshTokens.Repositories;
using MediatR;

namespace Inventory.Application.Auth.Commands.Logout;

/// <summary>
/// El logout real: revoca el refresh token en el servidor para que no se
/// pueda volver a usar, aunque todavía no haya expirado. Nunca falla de
/// cara al cliente — un token que ya no existe o ya estaba revocado
/// significa, para quien pidió logout, lo mismo: "listo, no hay sesión".
/// </summary>
public class LogoutCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IRefreshTokenGenerator refreshTokenGenerator,
    IUnitOfWork unitOfWork) : IRequestHandler<LogoutCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.RawToken))
        {
            var existing = await refreshTokenRepository.GetByTokenHashAsync(
                refreshTokenGenerator.Hash(request.RawToken));

            if (existing is not null && existing.IsActive(DateTime.UtcNow))
            {
                existing.Revoke(DateTime.UtcNow);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        return VoidResult.Instance;
    }
}
