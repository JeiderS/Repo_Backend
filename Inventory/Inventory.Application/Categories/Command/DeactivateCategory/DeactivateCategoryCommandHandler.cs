using Inventory.Application.Categories.Dto;
using Inventory.Application.Categories.Errors;
using Inventory.Domain.Categories.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Categories.Command.DeactivateCategory;

public class DeactivateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeactivateCategoryCommand, Result<CategoryDto, Error>>
{
    public async Task<Result<CategoryDto, Error>> Handle(DeactivateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id);
        if (category is null)
            return CategoryErrorBuilder.CategoryNotFound();

        // La entidad decide: si ya estaba inactiva, no hay nada que guardar.
        if (category.Deactivate())
            await unitOfWork.SaveChangesAsync(cancellationToken);

        return CategoryDto.FromEntity(category);
    }
}
