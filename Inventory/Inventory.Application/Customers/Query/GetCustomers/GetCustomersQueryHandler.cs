using Inventory.Application.Customers.Dto;
using Inventory.Domain.Customers.Repositories;
using MediatR;

namespace Inventory.Application.Customers.Query.GetCustomers;

public class GetCustomersQueryHandler(ICustomerRepository customerRepository)
    : IRequestHandler<GetCustomersQuery, IReadOnlyList<CustomerDto>>
{
    public async Task<IReadOnlyList<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await customerRepository.GetAllAsync();

        return customers.Select(CustomerDto.FromEntity).ToList();
    }
}
