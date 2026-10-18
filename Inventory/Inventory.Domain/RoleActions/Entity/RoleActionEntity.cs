using Inventory.Domain.Actions.Entity;
using Inventory.Domain.ModuleActions.Entity;
using Inventory.Domain.Roles.Entity;

namespace Inventory.Domain.RoleActions.Entity
{
    public class RoleActionEntity
    {
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
        public int ActionId { get; set; }

        public RoleEntity? Role { get; set; }
        public ModuleActionEntity ModuleAction { get; set; } = default!;
    }
}
