namespace Inventory.Domain.Customers.Entity
{
    public class CustomerEntity
    {
        // Para EF Core.
        private CustomerEntity() { }

        public int Id { get; private set; }
        public string Name { get; private set; } = default!;
        public string Phone { get; private set; } = default!;
        public string? Email { get; private set; }
        public string? Address { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>Regla de negocio: todo cliente nuevo empieza activo.</summary>
        public static CustomerEntity Create(string name, string phone, string? email, string? address) =>
            new()
            {
                Name = name,
                Phone = phone,
                Email = email,
                Address = address,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

        public void Update(string name, string phone, string? email, string? address)
        {
            Name = name;
            Phone = phone;
            Email = email;
            Address = address;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
