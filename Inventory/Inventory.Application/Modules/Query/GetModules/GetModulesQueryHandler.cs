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

        // Una sola consulta para las acciones de todos los módulos (evita N+1).
        var actionIdsByModule = await moduleRepository.GetActionIdsByModuleAsync();

        return modules
            .Select(module => ModuleDto.FromEntity(
                module,
                actionIdsByModule.TryGetValue(module.Id, out var actionIds) ? actionIds : Array.Empty<int>()))
            .ToList();
    }
}
