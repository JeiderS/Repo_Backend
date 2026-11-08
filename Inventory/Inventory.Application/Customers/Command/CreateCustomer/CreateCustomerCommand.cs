using Inventory.Application.Customers.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Customers.Command.CreateCustomer;

public class CreateCustomerCommand : IRequest<Result<CustomerDto, Error>>
{
    public string Name { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string? Email { get; set; }
    public string? Address { get; set; }
}
