using MediatR;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;


namespace Inventory.Application.Drivers.Commands.UpdateDrivers;

public record UpdateDriversCommand(int Id,
    string FirstName,
    string LastName,
    string SSN,
    DateTime DOB,
    string Address,
    string City,
    string Zip,
    string Phone,
    bool Active) : IRequest<Result<VoidResult, Error>>;