using Inventory.Application.Auth.Dto;
using Inventory.Application.Auth.Errors;
using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Users.DomainUsers;
using Inventory.Domain.Users.Entity;
using MediatR;

namespace Inventory.Application.Auth.Commands.Register;

public class RegisterCommandHandler(
    IUserCreateService userCreateService,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<RegisterCommand, Result<AuthResponseDto, Error>>
{
    public async Task<Result<AuthResponseDto, Error>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await userCreateService.EmailExistsAsync(request.Email))
            return AuthErrorBuilder.EmailAlreadyExists();

        var user = new UserEntity
        {
            Email = request.Email,
            PasswordHash = passwordHasher.Hash(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var profile = new UserProfileEntity
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Address = request.Address
        };

        var result = await userCreateService.CreateAsync(user, profile);
        if (!result.IsSuccess)
            return result.Error!;

        var createdUser = result.Value!;
        createdUser.Profile = profile;

        var token = jwtTokenGenerator.GenerateToken(createdUser);

        return new AuthResponseDto
        {
            Token = token,
            Email = createdUser.Email,
            FullName = $"{profile.FirstName} {profile.LastName}".Trim()
        };
    }
}
