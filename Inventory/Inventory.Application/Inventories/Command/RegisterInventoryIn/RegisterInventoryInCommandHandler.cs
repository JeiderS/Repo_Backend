using Inventory.Application.Inventories.Dto;
using Inventory.Application.Inventories.Errors;
using Inventory.Domain.Categories.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Inventories.Repositories;
using Inventory.Domain.InventoryMovements.Entity;
using Inventory.Domain.InventoryMovements.Repositories;
using Inventory.Domain.Products.Repositories;
using MediatR;

namespace Inventory.Application.Inventories.Command.RegisterInventoryIn;

public class RegisterInventoryInCommandHandler(
    IInventoryRepository inventoryRepository,
    IInventoryMovementRepository movementRepository,
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterInventoryInCommand, Result<InventoryOverviewDto, Error>>
{
    public async Task<Result<InventoryOverviewDto, Error>> Handle(RegisterInventoryInCommand request, CancellationToken cancellationToken)
    {
        return await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var inventory = await inventoryRepository.GetByIdAsync(request.InventoryId);
            if (inventory is null)
                return Result<InventoryOverviewDto, Error>.Failure(InventoryErrorBuilder.NotFound());

            var newQuantity = await inventoryRepository.TryIncreaseQuantityAsync(request.InventoryId, request.Quantity);
            if (newQuantity is null)
                return Result<InventoryOverviewDto, Error>.Failure(InventoryErrorBuilder.NotFound());

            var movement = InventoryMovementEntity.Create(
                request.InventoryId, InventoryMovementEntity.MovementTypeIn, request.Quantity, request.Description);
            await movementRepository.AddAsync(movement);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var product = await productRepository.GetByIdAsync(inventory.ProductId);
            var category = product is not null ? await categoryRepository.GetByIdAsync(product.CategoryId) : null;

            return Result<InventoryOverviewDto, Error>.Success(new InventoryOverviewDto
            {
                InventoryId = inventory.Id,
                ProductId = inventory.ProductId,
                ProductName = product?.Name ?? string.Empty,
                CategoryName = category?.Name ?? string.Empty,
                Quantity = newQuantity.Value,
            });
        });
    }
}
