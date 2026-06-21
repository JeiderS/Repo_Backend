using Inventory.Domain.Modules.DomainModules;
using Inventory.Domain.Modules.Models;
using MediatR;

namespace Inventory.Application.Modules.Query.GetModuleMenu;

public class GetModuleMenuQueryHandler(IModuleMenuService moduleMenuService)
    : IRequestHandler<GetModuleMenuQuery, IReadOnlyList<ModuleMenuItem>>
{
    public Task<IReadOnlyList<ModuleMenuItem>> Handle(GetModuleMenuQuery request, CancellationToken cancellationToken)
    {
        return moduleMenuService.GetMenuForUserAsync(request.UserId);
    }
}
