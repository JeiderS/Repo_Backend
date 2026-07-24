using Inventory.Application.Modules.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Modules.Command.DeactivateModule;

public record DeactivateModuleCommand(int Id) : IRequest<Result<ModuleDto, Error>>;
