using Inventory.Application.Modules.Dto;
using MediatR;

namespace Inventory.Application.Modules.Query.GetModules;

public record GetModulesQuery : IRequest<IReadOnlyList<ModuleDto>>;
