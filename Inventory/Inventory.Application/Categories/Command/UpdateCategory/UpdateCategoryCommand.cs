using Inventory.Application.Categories.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Categories.Command.UpdateCategory;

public class UpdateCategoryCommand : IRequest<Result<CategoryDto, Error>>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
