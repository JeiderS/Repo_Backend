using Inventory.Domain.Roles.Entity;
using Inventory.Domain.UserProfile.Entity;

namespace Inventory.Domain.Users.Entity
{
    public class UserEntity
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? RoleId { get; set; }
        public bool MustChangePassword { get; set; }

        public UserProfileEntity? Profile { get; set; }
        public RoleEntity? Role { get; set; }
    }
}
