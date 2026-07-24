using Inventory.Application.Modules.Dto;
using Inventory.Domain.Modules.DomainModules;
using MediatR;

namespace Inventory.Application.Modules.Query.GetModules;

public class GetModulesQueryHandler(IModuleListService moduleListService)
    : IRequestHandler<GetModulesQuery, IReadOnlyList<ModuleDto>>
{
    public async Task<IReadOnlyList<ModuleDto>> Handle(GetModulesQuery request, CancellationToken cancellationToken)
    {
        var modules = await moduleListService.GetAllAsync();
        return modules.Select(ModuleDto.FromEntity).ToList();
    }
}
