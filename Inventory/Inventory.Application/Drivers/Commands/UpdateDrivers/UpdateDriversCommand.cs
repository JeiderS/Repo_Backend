using MediatR;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;


namespace FleetManager.Application.Drivers.Commands.UpdateDrivers;

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