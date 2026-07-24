using Inventory.Domain.Modules.Entity;

namespace Inventory.Domain.Modules.DomainModules;

public interface IModuleGetByIdService
{
    Task<ModuleEntity?> GetByIdAsync(int id);
}
