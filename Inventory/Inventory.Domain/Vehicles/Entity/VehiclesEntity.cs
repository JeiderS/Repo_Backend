

namespace Inventory.Domain.Vehicles.Entity
{
    public class VehiclesEntity
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public string Make { get; set; }
        public string Capacity { get; set; }
        public bool Active { get; set; }
    }
}


