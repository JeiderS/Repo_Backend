using Inventory.Application.Categories.Dto;
using MediatR;

namespace Inventory.Application.Categories.Query.GetCategories;

public record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;
