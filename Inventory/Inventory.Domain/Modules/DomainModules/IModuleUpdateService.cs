namespace Inventory.Domain.Modules.DomainModules;

public interface IModuleUpdateService
{
    Task<bool> NameExistsForOtherAsync(string name, int excludeId);
}
