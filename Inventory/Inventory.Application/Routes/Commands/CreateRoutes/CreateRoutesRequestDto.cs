

namespace FleetManager.Application.Routes.Commands.CreateRoutes;

public record CreateRoutesRequestDto(
     int Id,
    string? Description,
    int DriverId,
    int VehicleId,
    string Origin,
    string Destination,
    TimeSpan StartTime,
    TimeSpan EndTime,
    bool Active
);
