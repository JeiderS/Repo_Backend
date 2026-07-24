using Inventory.Domain.Modules.Entity;

namespace Inventory.Domain.Modules.DomainModules;

public interface IModuleListService
{
    Task<IReadOnlyList<ModuleEntity>> GetAllAsync();
}
