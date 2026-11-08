using Inventory.Application.Customers.Dto;
using Inventory.Application.Customers.Errors;
using Inventory.Domain.Customers.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Customers.Command.SetCustomerStatus;

public class SetCustomerStatusCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SetCustomerStatusCommand, Result<CustomerDto, Error>>
{
    public async Task<Result<CustomerDto, Error>> Handle(SetCustomerStatusCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.Id);
        if (customer is null)
            return CustomerErrorBuilder.CustomerNotFound();

        if (request.IsActive)
            customer.Activate();
        else
            customer.Deactivate();

        // 0 filas guardadas NO es un error: si el estado no cambio, EF no
        // actualiza nada. Un fallo real llega como excepcion.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CustomerDto.FromEntity(customer);
    }
}
