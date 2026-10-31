using Inventory.Domain.InventoryMovements.Entity;

namespace Inventory.Domain.InventoryMovements.Repositories;

public interface IInventoryMovementRepository
{
    Task AddAsync(InventoryMovementEntity movement);

    /// <summary>Todos los movimientos, sin filtro (el cliente filtra si lo necesita).</summary>
    Task<List<InventoryMovementEntity>> GetAllAsync();
}
