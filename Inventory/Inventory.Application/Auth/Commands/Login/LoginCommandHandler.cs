using Inventory.Application.Auth.Dto;
using Inventory.Application.Auth.Errors;
using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.RefreshTokens.Entity;
using Inventory.Domain.RefreshTokens.Repositories;
using Inventory.Domain.Users.Entity;
using Inventory.Domain.Users.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Auth.Commands.Login;

/// <summary>
/// La "jugada" del login: buscar, verificar, preguntar si puede entrar,
/// generar el token y responder. Las "reglas del juego" (quién puede entrar,
/// cómo se arma el nombre completo, cuándo se bloquea) las responde la entidad.
/// </summary>
public class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    ILogger<LoginCommandHandler> logger) : IRequestHandler<LoginCommand, Result<AuthResponseDto, Error>>
{
    public async Task<Result<AuthResponseDto, Error>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;
        var user = await userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            // No se loguea la contraseña nunca; el email sí, para poder
            // detectar patrones de brute-force/credential-stuffing en logs.
            logger.LogWarning("Login fallido: no existe un usuario con email {Email}.", request.Email);
            return AuthErrorBuilder.InvalidCredentials();
        }

        // Si ya está bloqueado, ni se gasta en verificar la contraseña (evita
        // quemar ciclos de BCrypt en un intento que de entrada va a fallar).
        // La respuesta al cliente es la misma que credenciales inválidas —
        // nunca revelamos que la cuenta existe y está bloqueada (eso permite
        // enumerar cuentas reales probando 5 intentos por email candidato).
        // El detalle real sí queda en el log, que es interno.
        if (user.IsLockedOut(nowUtc))
        {
            logger.LogWarning("Login rechazado: {Email} sigue bloqueado hasta {LockedUntil}.", user.Email, user.LockedUntil);
            return AuthErrorBuilder.InvalidCredentials();
        }

        // Verificar la contraseña necesita un servicio externo (BCrypt), por eso
        // se hace aquí y no dentro de la entidad.
        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            // UPDATE atómico en la BD (no user.RegisterFailedLogin() +
            // SaveChanges): dos intentos fallidos concurrentes contra la
            // misma cuenta no pueden pisarse leyendo el mismo contador viejo.
            await userRepository.RegisterFailedLoginAtomicAsync(
                user.Id,
                nowUtc.Add(UserEntity.LockoutDuration),
                cancellationToken);

            logger.LogWarning("Login fallido: contraseña incorrecta para {Email}.", user.Email);
            return AuthErrorBuilder.InvalidCredentials();
        }

        if (!user.CanLogin())
        {
            logger.LogWarning("Login rechazado: {Email} está inactivo.", user.Email);
            return AuthErrorBuilder.InactiveUser();
        }

        user.RegisterSuccessfulLogin();

        var rawRefreshToken = refreshTokenGenerator.GenerateRawToken();
        var refreshToken = RefreshTokenEntity.Create(
            user.Id,
            refreshTokenGenerator.Hash(rawRefreshToken),
            nowUtc,
            nowUtc.Add(RefreshTokenPolicy.Lifetime));
        await refreshTokenRepository.AddAsync(refreshToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Un usuario tiene un único rol; GetByEmailAsync ya lo trae cargado.
        IReadOnlyList<string> roles = user.Role is null
            ? Array.Empty<string>()
            : new[] { user.Role.Name };

        var token = jwtTokenGenerator.GenerateToken(user, roles);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            Roles = roles,
            RefreshToken = rawRefreshToken
        };
    }
}
