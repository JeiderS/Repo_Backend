using Inventory.Application.Customers.Dto;
using Inventory.Domain.Customers.Entity;
using Inventory.Domain.Customers.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Customers.Command.CreateCustomer;

public class CreateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCustomerCommand, Result<CustomerDto, Error>>
{
    public async Task<Result<CustomerDto, Error>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // CustomerEntity.Create garantiza que el cliente nace activo.
        var customer = CustomerEntity.Create(request.Name, request.Phone, request.Email, request.Address);

        await customerRepository.AddAsync(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CustomerDto.FromEntity(customer);
    }
}
