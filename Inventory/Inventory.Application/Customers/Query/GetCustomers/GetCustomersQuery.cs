using Inventory.Application.Customers.Dto;
using MediatR;

namespace Inventory.Application.Customers.Query.GetCustomers;

public record GetCustomersQuery : IRequest<IReadOnlyList<CustomerDto>>;
