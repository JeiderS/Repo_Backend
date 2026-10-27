using Inventory.Application.Products.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Products.Command.SetProductStatus;

public class SetProductStatusCommand : IRequest<Result<ProductDto, Error>>
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}
