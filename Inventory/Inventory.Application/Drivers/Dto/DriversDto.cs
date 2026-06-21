
namespace Inventory.Application.Drivers.Dto;

public record DriversDto(
    int Id,
    string? FirstName,
    string? LastName,
    string? SSN,
    DateTime Dob,
    string? Address,
    string? City,
    string? Zip,
    string? Phone,
    bool Active
);

