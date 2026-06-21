using Inventory.Domain.Users.Entity;

namespace Inventory.Domain.Roles.Entity
{
    public class RoleEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
    }
}
