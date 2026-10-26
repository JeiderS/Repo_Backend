using Inventory.Application.Categories.Dto;
using Inventory.Application.Categories.Errors;
using Inventory.Domain.Categories.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Categories.Command.SetCategoryStatus;

public class SetCategoryStatusCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SetCategoryStatusCommand, Result<CategoryDto, Error>>
{
    public async Task<Result<CategoryDto, Error>> Handle(SetCategoryStatusCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id);
        if (category is null)
            return CategoryErrorBuilder.CategoryNotFound();

        if (request.IsActive)
            category.Activate();
        else
            category.Deactivate();

        // 0 filas guardadas NO es un error: si el estado no cambio, EF no
        // actualiza nada. Un fallo real llega como excepcion.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CategoryDto.FromEntity(category);
    }
}
