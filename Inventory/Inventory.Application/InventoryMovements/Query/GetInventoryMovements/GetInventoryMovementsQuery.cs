using Inventory.Application.InventoryMovements.Dto;
using MediatR;

namespace Inventory.Application.InventoryMovements.Query.GetInventoryMovements;

public class GetInventoryMovementsQuery : IRequest<IReadOnlyList<InventoryMovementDto>>
{
}
