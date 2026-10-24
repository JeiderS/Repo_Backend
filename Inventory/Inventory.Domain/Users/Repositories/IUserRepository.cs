using Inventory.Domain.Users.Entity;

namespace Inventory.Domain.Users.Repositories;

/// <summary>
/// Contrato para guardar y buscar usuarios. Domain dice QUÉ necesita;
/// Infrastructure decide CÓMO (EF Core, SQL Server...).
/// Reemplaza a las antiguas IUserCreateService, IUserGetByIdService,
/// IUserGetByEmailService, IUserListService, IUserStatusService,
/// IUserUpdateService e IUserProfileCreateService.
/// </summary>
public interface IUserRepository
{
    /// <summary>Usuario con su perfil y su rol, o null si no existe.</summary>
    Task<UserEntity?> GetByIdAsync(int id);

    /// <summary>Usuario con su perfil y su rol, o null si no existe.</summary>
    Task<UserEntity?> GetByEmailAsync(string email);

    Task<IReadOnlyList<UserEntity>> GetAllAsync();

    /// <summary>
    /// ¿Ya hay un usuario con ese email? Si se pasa <paramref name="excludeUserId"/>,
    /// ese usuario no cuenta (sirve al editar: tu propio email no es un duplicado).
    /// </summary>
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);

    /// <summary>Agrega el usuario (y su perfil, si lo tiene) a la unidad de trabajo.</summary>
    Task AddAsync(UserEntity user);

    /// <summary>Lectura liviana para el middleware: false si no existe.</summary>
    Task<bool> IsActiveAsync(int userId);

    /// <summary>Lectura liviana: el RoleId del usuario, o null si no tiene rol.</summary>
    Task<int?> GetRoleIdAsync(int userId);

    /// <summary>
    /// Registra un intento fallido de login como UPDATE atómico en una sola
    /// instrucción SQL (no pasa por el change tracker ni por un
    /// read-modify-write en memoria), para que dos intentos fallidos
    /// concurrentes contra la misma cuenta no se "pisen" y terminen
    /// contando solo uno. Si el contador llega a
    /// <see cref="Entity.UserEntity.MaxFailedLoginAttempts"/> con este
    /// intento, bloquea la cuenta hasta <paramref name="lockedUntilIfThresholdReached"/>
    /// y reinicia el contador a 0 (mismo comportamiento de negocio que
    /// <see cref="Entity.UserEntity.RegisterFailedLogin"/>, pero resuelto en la BD).
    /// </summary>
    Task RegisterFailedLoginAtomicAsync(
        int userId,
        DateTime lockedUntilIfThresholdReached,
        CancellationToken cancellationToken = default);
}
