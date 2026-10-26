using Inventory.Application.Categories.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Categories.Command.DeactivateCategory;

public record DeactivateCategoryCommand(int Id) : IRequest<Result<CategoryDto, Error>>;
