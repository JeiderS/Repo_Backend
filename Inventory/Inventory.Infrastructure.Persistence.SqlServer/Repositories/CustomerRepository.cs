using Inventory.Domain.Customers.Entity;
using Inventory.Domain.Customers.Repositories;
using Inventory.Infrastructure.Persistence.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.SqlServer.Repositories;

public class CustomerRepository(DataBaseContext context) : ICustomerRepository
{
    public async Task<CustomerEntity?> GetByIdAsync(int id)
    {
        return await context.Customers.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Customers.AnyAsync(c => c.Id == id);
    }

    public async Task<IReadOnlyList<CustomerEntity>> GetAllAsync()
    {
        return await context.Customers
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task AddAsync(CustomerEntity customer)
    {
        await context.Customers.AddAsync(customer);
    }
}
