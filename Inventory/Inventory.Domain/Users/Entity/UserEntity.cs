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
        // Constructor vacío y privado: solo lo usa EF Core para leer de la BD.
        private UserEntity() { }

        public int Id { get; private set; }
        public string Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int? RoleId { get; private set; }
        public bool MustChangePassword { get; private set; }

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
