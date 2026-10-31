using Inventory.Application.Inventories.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Inventories.Command.RegisterInventoryOut;

public class RegisterInventoryOutCommand : IRequest<Result<InventoryOverviewDto, Error>>
{
    public int InventoryId { get; set; }
    public decimal Quantity { get; set; }
    public string? Description { get; set; }
}
