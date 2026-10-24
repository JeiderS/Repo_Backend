using Inventory.Domain.Roles.Entity;
using Inventory.Domain.UserProfile.Entity;

namespace Inventory.Domain.Users.Entity
{
    /// <summary>
    /// Un usuario del sistema. Ya no es una "bolsa de datos": las propiedades
    /// solo se pueden cambiar desde adentro (private set), a través de métodos
    /// con nombre de negocio (Create, Deactivate, AssignRole...). Así nadie
    /// puede saltarse una regla haciendo user.IsActive = ... desde afuera.
    /// </summary>
    public class UserEntity
    {
        /// <summary>
        /// Intentos fallidos consecutivos que disparan el bloqueo. Pública
        /// porque IUserRepository.RegisterFailedLoginAtomicAsync la necesita
        /// para armar el UPDATE atómico — es la única fuente de verdad del
        /// umbral, nadie debe hardcodear el número de nuevo.
        /// </summary>
        public const int MaxFailedLoginAttempts = 5;

        /// <summary>Duración fija del bloqueo por intentos fallidos.</summary>
        public static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

        // Constructor vacío y privado: solo lo usa EF Core para leer de la BD.
        private UserEntity() { }

        public int Id { get; private set; }
        public string Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int? RoleId { get; private set; }
        public bool MustChangePassword { get; private set; }
        public int FailedLoginAttempts { get; private set; }
        public DateTime? LockedUntil { get; private set; }

        public UserProfileEntity? Profile { get; private set; }
        public RoleEntity? Role { get; private set; }

        /// <summary>Nombre completo según el perfil, o null si no tiene perfil.</summary>
        public string? FullName => Profile?.FullName;

        /// <summary>
        /// Crea un usuario nuevo aplicando las reglas de nacimiento:
        /// - Todo usuario nuevo empieza ACTIVO.
        /// - Debe cambiar su contraseña al entrar, salvo que se indique lo contrario
        ///   (spec: user-administration, "Admin User Creation").
        /// </summary>
        public static UserEntity Create(
            string email,
            string passwordHash,
            UserProfileEntity? profile,
            RoleEntity? role,
            bool? mustChangePassword,
            DateTime createdAtUtc)
        {
            var user = new UserEntity
            {
                Email = email,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = createdAtUtc,
                MustChangePassword = mustChangePassword ?? true,
                Profile = profile
            };

            user.AssignRole(role);
            return user;
        }

        /// <summary>Regla de negocio: solo un usuario activo puede iniciar sesión.</summary>
        public bool CanLogin() => IsActive;

        /// <summary>¿Sigue bloqueado por intentos fallidos a esta hora?</summary>
        public bool IsLockedOut(DateTime nowUtc) => LockedUntil is not null && LockedUntil > nowUtc;

        /// <summary>
        /// Cuenta un intento fallido. Al llegar a <see cref="MaxFailedLoginAttempts"/>
        /// bloquea la cuenta por <see cref="LockoutDuration"/> y reinicia el contador,
        /// para que al desbloquearse el usuario tenga de nuevo los intentos completos.
        /// </summary>
        public void RegisterFailedLogin(DateTime nowUtc)
        {
            FailedLoginAttempts++;

            if (FailedLoginAttempts >= MaxFailedLoginAttempts)
            {
                LockedUntil = nowUtc.Add(LockoutDuration);
                FailedLoginAttempts = 0;
            }
        }

        /// <summary>Un login exitoso borra cualquier historial de intentos fallidos.</summary>
        public void RegisterSuccessfulLogin()
        {
            FailedLoginAttempts = 0;
            LockedUntil = null;
        }

        public void Activate() => IsActive = true;

        /// <summary>
        /// Sin auto-protección (spec: role-permission-management, "No Self-Protection
        /// on Role Removal" aplica el mismo principio): un admin puede desactivar
        /// su propia cuenta.
        /// </summary>
        public void Deactivate() => IsActive = false;

        public void ChangeEmail(string email) => Email = email;

        /// <summary>
        /// Asigna un rol, REEMPLAZANDO el anterior: un usuario tiene un único rol
        /// (spec: user-administration, "Reassigning a role replaces the previous
        /// one"). null deja al usuario sin rol.
        /// </summary>
        /// <returns>true si el rol cambió (útil para invalidar cachés).</returns>
        public bool AssignRole(RoleEntity? role)
        {
            var newRoleId = role?.Id;
            var changed = RoleId != newRoleId;

            RoleId = newRoleId;
            Role = role;

            return changed;
        }
    }
}
