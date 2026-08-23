using Inventory.Domain.Actions.Entity;
using Inventory.Domain.Roles.Entity;

namespace Inventory.Domain.RoleActions.Entity
{
    public class RoleActionEntity
    {
        public int RoleId { get; set; }
        public int ActionId { get; set; }

        public RoleEntity? Role { get; set; }
        public ActionEntity? Action { get; set; }
    }
}
