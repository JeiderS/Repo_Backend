using Inventory.Domain.SaleDetails.Entity;

namespace Inventory.Domain.SaleDetails.Repositories;

public interface ISaleDetailRepository
{
    Task AddAsync(SaleDetailEntity detail);

    Task<List<SaleDetailEntity>> GetBySaleIdAsync(int saleId);
}
