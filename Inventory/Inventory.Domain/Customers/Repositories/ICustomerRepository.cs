using Inventory.Domain.Customers.Entity;

namespace Inventory.Domain.Customers.Repositories;

public interface ICustomerRepository
{
    Task<CustomerEntity?> GetByIdAsync(int id);

    Task<bool> ExistsAsync(int id);

    /// <summary>Todos los clientes (activos e inactivos), ordenados por nombre.</summary>
    Task<IReadOnlyList<CustomerEntity>> GetAllAsync();

    Task AddAsync(CustomerEntity customer);
}
