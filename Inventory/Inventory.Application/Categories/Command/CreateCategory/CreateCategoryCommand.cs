using Inventory.Application.Categories.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Categories.Command.CreateCategory;

public class CreateCategoryCommand : IRequest<Result<CategoryDto, Error>>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
