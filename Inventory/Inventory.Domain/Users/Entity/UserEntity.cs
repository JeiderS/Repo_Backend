namespace Inventory.Domain.Users.Entity
{
    public class UserEntity
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserProfileEntity? Profile { get; set; }
    }
}
