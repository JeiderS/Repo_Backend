using Inventory.Application.Auth.Dto;
using Inventory.Application.Auth.Errors;
using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Users.DomainUsers;
using Inventory.Domain.Users.Repositories;
using MediatR;

namespace Inventory.Application.Auth.Commands.Login;

/// <summary>
/// La "jugada" del login: buscar, verificar, preguntar si puede entrar,
/// generar el token y responder. Las "reglas del juego" (quién puede entrar,
/// cómo se arma el nombre completo) las responde la entidad.
/// </summary>
public class LoginCommandHandler(
    IUserRepository userRepository,
    IUserAuthorizationService userAuthorizationService,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginCommand, Result<AuthResponseDto, Error>>
{
    public async Task<Result<AuthResponseDto, Error>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);

        // Verificar la contraseña necesita un servicio externo (BCrypt), por eso
        // se hace aquí y no dentro de la entidad.
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            return AuthErrorBuilder.InvalidCredentials();

        if (!user.CanLogin())
            return AuthErrorBuilder.InactiveUser();

        var roles = await userAuthorizationService.GetRolesAsync(user.Id);

        var token = jwtTokenGenerator.GenerateToken(user, roles);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            Roles = roles
        };
    }
}
