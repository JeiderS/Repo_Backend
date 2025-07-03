


namespace FleetManager.Domain.Routes.Entity
{
    public class RoutesEntity
    {
    public int Id { get; set; }
    public string Description { get; set; }
    public int DriverId { get; set; }
    public int VehicleId { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool Active { get; set; }
    }
}