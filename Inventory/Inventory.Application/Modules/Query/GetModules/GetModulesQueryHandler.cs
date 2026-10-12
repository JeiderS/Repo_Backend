using Inventory.Application.Modules.Dto;
using Inventory.Domain.Modules.Repositories;
using MediatR;

namespace Inventory.Application.Modules.Query.GetModules;

public class GetModulesQueryHandler(IModuleRepository moduleRepository)
    : IRequestHandler<GetModulesQuery, IReadOnlyList<ModuleDto>>
{
    public async Task<IReadOnlyList<ModuleDto>> Handle(GetModulesQuery request, CancellationToken cancellationToken)
    {
        var modules = await moduleRepository.GetAllAsync();
        return modules.Select(ModuleDto.FromEntity).ToList();
    }
}
