using Inventory.Application.Common.Interfaces;
using Inventory.Application.Users.Dto;
using Inventory.Application.Users.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Roles.DomainRoles;
using Inventory.Domain.Roles.Entity;
using Inventory.Domain.UserProfile.DomainUserProfile;
using Inventory.Domain.UserProfile.Entity;
using Inventory.Domain.Users.DomainUsers;
using Inventory.Domain.Users.Entity;
using MediatR;

namespace Inventory.Application.Users.Command.CreateUser;

public class CreateUserCommandHandler(
    IUserCreateService userCreateService,
    IUserProfileCreateService userProfileCreateService,
    IRoleGetByIdService roleGetByIdService,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateUserCommand, Result<UserDto, Error>>
{
    public async Task<Result<UserDto, Error>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await userCreateService.EmailExistsAsync(request.Email))
            return UserErrorBuilder.EmailAlreadyExists();

        RoleEntity? role = null;
        if (request.RoleId is not null)
        {
            role = await roleGetByIdService.GetByIdAsync(request.RoleId.Value);
            if (role is null)
                return UserErrorBuilder.RoleNotFound();
        }

        var user = new UserEntity
        {
            Email = request.Email,
            PasswordHash = passwordHasher.Hash(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            RoleId = request.RoleId,
            Role = role,
            // Por defecto true a menos que el request lo desactive explícitamente
            // (spec: user-administration, "Admin User Creation").
            MustChangePassword = request.MustChangePassword ?? true
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

        await userCreateService.AddAsync(user);
        await userProfileCreateService.AddAsync(profile);

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (savedRows <= 0)
            return UserErrorBuilder.CreationException();

        return UserDto.FromEntity(user);
    }
}
