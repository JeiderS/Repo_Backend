using Inventory.Application.Inventories.Dto;
using MediatR;

namespace Inventory.Application.Inventories.Query.GetInventoryOverview;

public class GetInventoryOverviewQuery : IRequest<IReadOnlyList<InventoryOverviewDto>>
{
}
