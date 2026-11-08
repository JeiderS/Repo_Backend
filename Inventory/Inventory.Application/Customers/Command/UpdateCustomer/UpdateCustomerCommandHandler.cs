using Inventory.Application.Customers.Dto;
using Inventory.Application.Customers.Errors;
using Inventory.Domain.Customers.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Customers.Command.UpdateCustomer;

public class UpdateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCustomerCommand, Result<CustomerDto, Error>>
{
    public async Task<Result<CustomerDto, Error>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.Id);
        if (customer is null)
            return CustomerErrorBuilder.CustomerNotFound();

        customer.Update(request.Name, request.Phone, request.Email, request.Address);

        // 0 filas guardadas NO es un error: si los datos no cambiaron, EF no
        // actualiza nada. Un fallo real llega como excepcion.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CustomerDto.FromEntity(customer);
    }
}
