using Inventory.Application.Categories.Dto;
using Inventory.Domain.Categories.Repositories;
using MediatR;

namespace Inventory.Application.Categories.Query.GetCategories;

public class GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync();

        return categories.Select(CategoryDto.FromEntity).ToList();
    }
}
