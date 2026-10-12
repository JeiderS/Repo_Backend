using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Domain.Modules.Entity
{
    public class ModuleEntity
    {
        // Para EF Core.
        private ModuleEntity() { }

        public int Id { get; private set; }
        public string Name { get; private set; } = default!;
        public string? Icon { get; private set; }
        public string? Route { get; private set; }
        public int? ParentId { get; private set; }
        public bool IsActive { get; private set; }
        public int SortOrder { get; private set; }

        /// <summary>Solo se activa por SQL: ningún método de la app lo cambia.</summary>
        public bool RequiresSystemAdmin { get; private set; }

        public ModuleEntity? Parent { get; private set; }
        public ICollection<ModuleEntity> Children { get; private set; } = new List<ModuleEntity>();

        /// <summary>Regla de negocio: todo módulo nuevo empieza activo.</summary>
        public static ModuleEntity Create(string name, string? icon, string? route, int? parentId, int sortOrder) =>
            new()
            {
                Name = name,
                Icon = icon,
                Route = route,
                ParentId = parentId,
                SortOrder = sortOrder,
                IsActive = true
            };

        /// <summary>
        /// Actualiza los datos del módulo. Regla de negocio: un módulo no puede
        /// ser padre de sí mismo. (Que el padre EXISTA lo comprueba Application,
        /// porque necesita consultar la base de datos.)
        /// </summary>
        public Result<VoidResult, Error> Update(string name, string? icon, string? route, int? parentId, int sortOrder)
        {
            if (parentId is not null && parentId == Id)
                return ModuleErrors.CannotBeItsOwnParent();

            Name = name;
            Icon = icon;
            Route = route;
            ParentId = parentId;
            SortOrder = sortOrder;

            return VoidResult.Instance;
        }

        /// <summary>
        /// Regla de negocio (design.md D5): un módulo es visible para un rol si
        /// el rol tiene AL MENOS UNA de sus acciones concedida y, cuando el
        /// módulo requiere System Admin, el rol además es System Admin.
        /// Tener todas las acciones NO alcanza para un módulo de System Admin.
        /// </summary>
        public bool IsVisibleTo(bool roleHasAnyActionOfThisModule, bool roleIsSystemAdmin) =>
            roleHasAnyActionOfThisModule && (!RequiresSystemAdmin || roleIsSystemAdmin);

        /// <summary>
        /// Regla de negocio: desactivar un módulo ya inactivo no hace nada.
        /// </summary>
        /// <returns>true si el estado cambió; false si ya estaba inactivo.</returns>
        public bool Deactivate()
        {
            if (!IsActive)
                return false;

            IsActive = false;
            return true;
        }
    }
}
