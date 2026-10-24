using Inventory.Application.Auth.Dto;
using Inventory.Application.Auth.Errors;
using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.RefreshTokens.Entity;
using Inventory.Domain.RefreshTokens.Repositories;
using Inventory.Domain.Users.Repositories;
using MediatR;

namespace Inventory.Application.Auth.Commands.RefreshToken;

/// <summary>
/// Rota la sesión: cambia un refresh token vigente por uno nuevo y entrega
/// un access token nuevo. El viejo queda revocado, así que solo se puede
/// usar una vez (si alguien más lo usara después, ya no funcionaría).
/// </summary>
public class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IRefreshTokenGenerator refreshTokenGenerator,
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IUnitOfWork unitOfWork) : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto, Error>>
{
    public async Task<Result<AuthResponseDto, Error>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;

        var existing = await refreshTokenRepository.GetByTokenHashAsync(
            refreshTokenGenerator.Hash(request.RawToken));

        if (existing is null || !existing.IsActive(nowUtc))
            return AuthErrorBuilder.InvalidRefreshToken();

        // Un usuario desactivado no puede renovar su sesión aunque su
        // refresh token siga siendo técnicamente válido (misma regla que ya
        // aplica al JWT corto: "Immediate Deactivation Enforcement").
        var user = await userRepository.GetByIdAsync(existing.UserId);
        if (user is null || !user.CanLogin())
            return AuthErrorBuilder.InvalidRefreshToken();

        existing.Revoke(nowUtc);

        var rawRefreshToken = refreshTokenGenerator.GenerateRawToken();
        var newRefreshToken = RefreshTokenEntity.Create(
            user.Id,
            refreshTokenGenerator.Hash(rawRefreshToken),
            nowUtc,
            nowUtc.Add(RefreshTokenPolicy.Lifetime));
        await refreshTokenRepository.AddAsync(newRefreshToken);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyConflictException)
        {
            // Otro request ya rotó este mismo token mientras procesábamos
            // este — señal de uso indebido (ej. token robado usado en
            // paralelo), no un problema transitorio: no se reintenta.
            return AuthErrorBuilder.InvalidRefreshToken();
        }

        IReadOnlyList<string> roles = user.Role is null
            ? Array.Empty<string>()
            : new[] { user.Role.Name };

        return new AuthResponseDto
        {
            Token = jwtTokenGenerator.GenerateToken(user, roles),
            Email = user.Email,
            FullName = user.FullName,
            Roles = roles,
            RefreshToken = rawRefreshToken
        };
    }
}
