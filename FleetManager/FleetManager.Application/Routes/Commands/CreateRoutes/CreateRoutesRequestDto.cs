

namespace FleetManager.Application.Routes.Commands.CreateRoutes;

public record CreateRoutesRequestDto(
    int Id,
    string? Description,
    int DriverId,
    int VehicleId,
    bool Active
);
