


namespace Fleet.Domain.Routes.Entity
{
    public class RoutesEntity
    {
    public int Id { get; set; }
    public string Description { get; set; }
    public int DriverId { get; set; }
    public int VehicleId { get; set; }
    public bool Active { get; set; }
    }
}