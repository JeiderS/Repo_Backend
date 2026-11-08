using Inventory.Application.Customers.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Customers.Command.SetCustomerStatus;

public class SetCustomerStatusCommand : IRequest<Result<CustomerDto, Error>>
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}
