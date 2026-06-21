using Inventory.Domain.Modules.Models;
using MediatR;

namespace Inventory.Application.Modules.Query.GetModuleMenu;

public record GetModuleMenuQuery(int UserId) : IRequest<IReadOnlyList<ModuleMenuItem>>;
