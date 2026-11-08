using Inventory.Application.Customers.Dto;
using Inventory.Application.Customers.Errors;
using Inventory.Domain.Customers.Repositories;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Customers.Query.GetCustomerById;

public class GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto, Error>>
{
    public async Task<Result<CustomerDto, Error>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.Id);
        if (customer is null)
            return CustomerErrorBuilder.CustomerNotFound();

        return CustomerDto.FromEntity(customer);
    }
}
