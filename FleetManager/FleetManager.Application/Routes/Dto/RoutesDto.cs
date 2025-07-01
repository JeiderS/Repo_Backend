

namespace FleetManager.Application.Routes.Dto
{
    public class RoutesDto
    {
    public int Id { get; set; }
    public string? Description { get; set; }
    public int DriverId { get; set; }
    public int VehicleId { get; set; }
    public bool Active { get; set; }
    }
}