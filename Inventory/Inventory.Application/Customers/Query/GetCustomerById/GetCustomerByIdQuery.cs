using Inventory.Application.Customers.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Customers.Query.GetCustomerById;

public record GetCustomerByIdQuery(int Id) : IRequest<Result<CustomerDto, Error>>;
