using Inventory.Application.Categories.Dto;
using Inventory.Application.Categories.Errors;
using Inventory.Domain.Categories.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Categories.Command.UpdateCategory;

public class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto, Error>>
{
    public async Task<Result<CategoryDto, Error>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id);
        if (category is null)
            return CategoryErrorBuilder.CategoryNotFound();

        if (await categoryRepository.NameExistsAsync(request.Name, excludeCategoryId: request.Id))
            return CategoryErrorBuilder.NameAlreadyExists();

        category.Update(request.Name, request.Description);

        // 0 filas guardadas NO es un error: si los datos no cambiaron, EF no
        // actualiza nada. Un fallo real llega como excepcion.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CategoryDto.FromEntity(category);
    }
}
