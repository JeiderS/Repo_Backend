using Inventory.Domain.Products.Entity;

namespace Inventory.Domain.Products.Repositories;

public interface IProductRepository
{
    Task<ProductEntity?> GetByIdAsync(int id);

    Task<bool> ExistsAsync(int id);

    /// <summary>Todos los productos (activos e inactivos), ordenados por nombre.</summary>
    Task<IReadOnlyList<ProductEntity>> GetAllAsync();

    Task AddAsync(ProductEntity product);
}
