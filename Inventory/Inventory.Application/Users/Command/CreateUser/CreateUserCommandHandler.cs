using Inventory.Application.Common.Interfaces;
using Inventory.Application.Users.Dto;
using Inventory.Application.Users.Errors;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Roles.Entity;
using Inventory.Domain.Roles.Repositories;
using Inventory.Domain.UserProfile.Entity;
using Inventory.Domain.Users.Entity;
using Inventory.Domain.Users.Repositories;
using MediatR;

namespace Inventory.Application.Users.Command.CreateUser;

/// <summary>
/// Caso de uso "crear usuario". Solo ORQUESTA: comprueba lo que necesita la
/// base de datos (email repetido, rol existente) y deja que la entidad aplique
/// sus propias reglas (empieza activo, debe cambiar la contraseña...).
/// </summary>
public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateUserCommand, Result<UserDto, Error>>
{
    public async Task<Result<UserDto, Error>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Regla de negocio que necesita ver a TODOS los usuarios: se comprueba
        // aquí. El índice único de la BD es la garantía final.
        if (await userRepository.EmailExistsAsync(request.Email))
            return UserErrorBuilder.EmailAlreadyExists();

        RoleEntity? role = null;
        if (request.RoleId is not null)
        {
            role = await roleRepository.GetByIdAsync(request.RoleId.Value);
            if (role is null)
                return UserErrorBuilder.RoleNotFound();
        }

        var profile = UserProfileEntity.Create(request.FirstName, request.LastName, request.Phone, request.Address);

        var user = UserEntity.Create(
            request.Email,
            passwordHasher.Hash(request.Password),
            profile,
            role,
            request.MustChangePassword,
            DateTime.UtcNow);

        // Al agregar el usuario, EF agrega también su perfil (user.Profile).
        await userRepository.AddAsync(user);

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (savedRows <= 0)
            return UserErrorBuilder.CreationException();

        return UserDto.FromEntity(user);
    }
}
