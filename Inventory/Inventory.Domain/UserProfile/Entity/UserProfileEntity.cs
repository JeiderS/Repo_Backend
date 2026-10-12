using Inventory.Domain.Users.Entity;

namespace Inventory.Domain.UserProfile.Entity
{
    public class UserProfileEntity
    {
        // Para EF Core.
        private UserProfileEntity() { }

        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string FirstName { get; private set; } = default!;
        public string LastName { get; private set; } = default!;
        public string? PhotoUrl { get; private set; }
        public string? Phone { get; private set; }
        public string? Address { get; private set; }

        public UserEntity? User { get; private set; }

        /// <summary>Regla de negocio: el nombre completo es nombre + apellido.</summary>
        public string FullName => $"{FirstName} {LastName}".Trim();

        public static UserProfileEntity Create(string firstName, string lastName, string? phone, string? address) =>
            new()
            {
                FirstName = firstName,
                LastName = lastName,
                Phone = phone,
                Address = address
            };

        public void Update(string firstName, string lastName, string? phone, string? address)
        {
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Address = address;
        }
    }
}
