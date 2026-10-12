namespace Inventory.Domain.Roles.Entity
{
    public class RoleEntity
    {
        // Para EF Core.
        private RoleEntity() { }

        public int Id { get; private set; }
        public string Name { get; private set; } = default!;

        /// <summary>
        /// Solo se activa por SQL (design.md D1). No existe ningún método que lo
        /// cambie: así es IMPOSIBLE que la aplicación cree o convierta un rol en
        /// administrador del sistema.
        /// </summary>
        public bool IsSystemAdmin { get; private set; }

        /// <summary>Regla de negocio: un rol creado desde la aplicación nunca es System Admin.</summary>
        public static RoleEntity Create(string name) => new() { Name = name, IsSystemAdmin = false };

        public void Rename(string name) => Name = name;
    }
}
