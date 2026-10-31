using Inventory.Domain.Sales.Entity;

namespace Inventory.Domain.Sales.Repositories;

public interface ISaleRepository
{
    Task<SaleEntity?> GetByIdAsync(int id);

    /// <summary>Todas las ventas, sin filtro (el cliente filtra/pagina si lo necesita).</summary>
    Task<List<SaleEntity>> GetAllAsync();

    Task AddAsync(SaleEntity sale);
}
