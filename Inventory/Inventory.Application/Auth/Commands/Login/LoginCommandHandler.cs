using Inventory.Application.Auth.Dto;
using Inventory.Application.Auth.Errors;
using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Users.DomainUsers;
using MediatR;

namespace Inventory.Application.Auth.Commands.Login;

public class LoginCommandHandler(
    IUserGetByEmailService userGetByEmailService,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginCommand, Result<AuthResponseDto, Error>>
{
    public async Task<Result<AuthResponseDto, Error>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userGetByEmailService.GetByEmailAsync(request.Email);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            return AuthErrorBuilder.InvalidCredentials();

        if (!user.IsActive)
            return AuthErrorBuilder.InactiveUser();

        var token = jwtTokenGenerator.GenerateToken(user);

        var fullName = user.Profile is null
            ? null
            : $"{user.Profile.FirstName} {user.Profile.LastName}".Trim();

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            FullName = fullName
        };
    }
}
