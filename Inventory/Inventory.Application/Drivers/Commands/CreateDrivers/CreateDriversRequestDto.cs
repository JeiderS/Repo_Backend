

namespace Inventory.Application.Drivers.Commands.CreateDrivers;

public record CreateDriversRequestDto(
    string FirstName,
    string LastName,
    string SSN,
    DateTime Dob,
    string? Address,
    string? City,
    string? Zip,
    string? Phone,
    bool Active
);
