namespace Inventory.Application.Vehicles.Commands.CreateVehicles;

public record CreateVehiclesRequestDto(
    int Id,
    string? Description,
    int Year,
    string? Make,
    string? Capacity,
    bool Active
);
