using Inventory.Application.Categories.Dto;
using Inventory.Application.Categories.Errors;
using Inventory.Domain.Categories.Entity;
using Inventory.Domain.Categories.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Categories.Command.CreateCategory;

public class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoryCommand, Result<CategoryDto, Error>>
{
    public async Task<Result<CategoryDto, Error>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (await categoryRepository.NameExistsAsync(request.Name))
            return CategoryErrorBuilder.NameAlreadyExists();

        // CategoryEntity.Create garantiza que la categoria nace activa.
        var category = CategoryEntity.Create(request.Name, request.Description);

        await categoryRepository.AddAsync(category);

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (savedRows <= 0)
            return CategoryErrorBuilder.CreationException();

        return CategoryDto.FromEntity(category);
    }
}
