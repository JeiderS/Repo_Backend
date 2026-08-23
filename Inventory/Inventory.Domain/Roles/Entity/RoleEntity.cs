namespace Inventory.Domain.Roles.Entity
{
    public class RoleEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public bool IsSystemAdmin { get; set; }
    }
}
