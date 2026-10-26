using Inventory.Application.Categories.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Categories.Command.SetCategoryStatus;

public class SetCategoryStatusCommand : IRequest<Result<CategoryDto, Error>>
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}
