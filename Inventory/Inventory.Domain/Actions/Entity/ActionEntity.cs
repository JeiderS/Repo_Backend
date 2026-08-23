using Inventory.Domain.Modules.Entity;

namespace Inventory.Domain.Actions.Entity
{
    public class ActionEntity
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public required string Code { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; }

        public ModuleEntity? Module { get; set; }
    }
}
