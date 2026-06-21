using MediatR;
using Inventory.Application.Drivers.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Drivers.Query.GetDriversById;
public record GetDriversByIdQuery (int Id) : IRequest<Result<DriversDto, Error>>;