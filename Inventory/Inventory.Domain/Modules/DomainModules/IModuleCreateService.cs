using Inventory.Domain.Modules.Entity;

namespace Inventory.Domain.Modules.DomainModules;

public interface IModuleCreateService
{
    Task<bool> NameExistsAsync(string name);
    Task AddAsync(ModuleEntity module);
}
