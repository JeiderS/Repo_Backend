using Inventory.Domain.Categories.Entity;

namespace Inventory.Domain.Categories.Repositories;

public interface ICategoryRepository
{
    Task<CategoryEntity?> GetByIdAsync(int id);

    Task<bool> ExistsAsync(int id);

    /// <summary>Todas las categorías (activas e inactivas), ordenadas por nombre.</summary>
    Task<IReadOnlyList<CategoryEntity>> GetAllAsync();

    /// <summary>¿Ya hay una categoría con ese nombre? (ignorando <paramref name="excludeCategoryId"/>).</summary>
    Task<bool> NameExistsAsync(string name, int? excludeCategoryId = null);

    Task AddAsync(CategoryEntity category);
}
