namespace Inventory.Domain.Users.DomainUsers;

public interface IUserUpdateService
{
    Task<bool> EmailExistsForOtherAsync(string email, int excludeId);
}
