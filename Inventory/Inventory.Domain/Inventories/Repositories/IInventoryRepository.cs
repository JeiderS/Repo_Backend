using Inventory.Domain.Inventories.Entity;

namespace Inventory.Domain.Inventories.Repositories;

public interface IInventoryRepository
{
    Task<InventoryEntity?> GetByIdAsync(int id);

    /// <summary>Todos los inventarios, uno por producto.</summary>
    Task<List<InventoryEntity>> GetAllAsync();

    Task AddAsync(InventoryEntity inventory);

    /// <summary>
    /// Suma <paramref name="amount"/> a la cantidad actual de forma atómica
    /// (UPDATE ... SET Quantity = Quantity + amount) y devuelve la cantidad
    /// resultante ya persistida, o null si el inventario indicado no existe.
    /// El valor devuelto se lee con una consulta sin tracking aparte, porque
    /// ExecuteUpdateAsync no refresca la instancia ya cargada por el change
    /// tracker (ver TryDecreaseQuantityAsync) — nunca calcular el nuevo valor
    /// como "entidadYaCargada.Quantity + amount" en el handler.
    /// </summary>
    Task<decimal?> TryIncreaseQuantityAsync(int inventoryId, decimal amount);

    /// <summary>
    /// Resta <paramref name="amount"/> a la cantidad actual de forma atómica,
    /// solo si hay suficiente stock, y devuelve la cantidad resultante ya
    /// persistida, o null si no hay suficiente stock (o el inventario no
    /// existe). El WHERE (Quantity &gt;= amount) es la defensa completa contra
    /// condiciones de carrera entre dos salidas concurrentes: si el stock ya
    /// fue consumido por otra petición, 0 filas coinciden y el método
    /// devuelve null. NO reemplazar por un patrón de cargar-entidad-y-guardar:
    /// eso reintroduce la carrera. El valor devuelto se re-lee sin tracking
    /// después del UPDATE — ExecuteUpdateAsync bypasea el change tracker, así
    /// que una entidad ya cargada antes de esta llamada queda con su
    /// Quantity vieja; no usarla para calcular el resultado.
    /// </summary>
    Task<decimal?> TryDecreaseQuantityAsync(int inventoryId, decimal amount);
}
