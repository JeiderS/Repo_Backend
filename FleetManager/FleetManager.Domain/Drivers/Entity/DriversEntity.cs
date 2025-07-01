

namespace FleetManager.Domain.Drivers.Entity
{
    public class DriversEntity
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string SSN { get; set; }
        public DateTime Dob { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string Zip { get; set; }
        public required string Phone { get; set; }
        public bool Active { get; set; }
    }
}
