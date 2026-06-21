using Inventory.Domain.Modules.Entity;
using Inventory.Domain.Roles.Entity;

namespace Inventory.Domain.RoleModules.Entity
{
    public class RoleModuleEntity
    {
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }

        public RoleEntity? Role { get; set; }
        public ModuleEntity? Module { get; set; }
    }
}
