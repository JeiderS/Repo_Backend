namespace Inventory.Domain.Modules.Entity
{
    public class ModuleEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Icon { get; set; }
        public string? Route { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public bool RequiresSystemAdmin { get; set; }

        public ModuleEntity? Parent { get; set; }
        public ICollection<ModuleEntity> Children { get; set; } = new List<ModuleEntity>();
    }
}
