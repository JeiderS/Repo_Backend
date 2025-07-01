using MediatR;
using FleetManager.Application.Drivers.Dto;
using FleetManager.Domain.Common.Results;
using FleetManager.Domain.Common.Results.Errors;

namespace FleetManager.Application.Drivers.Query.GetDriversById;
public record GetDriversByIdQuery (int Id) : IRequest<Result<DriversDto, Error>>;