namespace Inventory.Domain.Users.Entity
{
    public class UserProfileEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
