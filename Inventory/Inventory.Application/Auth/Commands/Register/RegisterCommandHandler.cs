using Inventory.Application.Auth.Dto;
using Inventory.Application.Auth.Errors;
using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.UserProfile.DomainUserProfile;
using Inventory.Domain.UserProfile.Entity;
using Inventory.Domain.Users.DomainUsers;
using Inventory.Domain.Users.Entity;
using MediatR;

namespace Inventory.Application.Auth.Commands.Register;

public class RegisterCommandHandler(
    IUserCreateService userCreateService,
    IUserProfileCreateService userProfileCreateService,
    IUnitOfWork unitOfWork,
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
            Address = request.Address,
            User = user
        };
        user.Profile = profile;

        // Ambos Add se acumulan en el mismo DbContext (scoped); un único
        // SaveChangesAsync los persiste en una sola transacción. EF resuelve
        // el FK UserId del perfil con el Id generado para el usuario gracias
        // a la navegación configurada en UserConfiguration.
        await userCreateService.AddAsync(user);
        await userProfileCreateService.AddAsync(profile);

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (savedRows <= 0)
            return AuthErrorBuilder.RegistrationException();

        // El usuario nuevo aún no tiene roles asignados (se asignan luego vía UserRoles).
        var token = jwtTokenGenerator.GenerateToken(user, Array.Empty<string>());

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            FullName = $"{profile.FirstName} {profile.LastName}".Trim(),
            Roles = Array.Empty<string>()
        };
    }
}
