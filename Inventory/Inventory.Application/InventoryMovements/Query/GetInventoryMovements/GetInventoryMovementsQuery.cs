using Inventory.Application.InventoryMovements.Dto;
using MediatR;

namespace Inventory.Application.InventoryMovements.Query.GetInventoryMovements;

public class GetInventoryMovementsQuery : IRequest<IReadOnlyList<InventoryMovementDto>>
{
    /// <summary>Si se indica, solo se devuelven los movimientos de ese inventario.</summary>
    public int? InventoryId { get; set; }
}
