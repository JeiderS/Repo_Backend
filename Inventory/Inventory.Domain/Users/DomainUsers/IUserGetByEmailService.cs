using Inventory.Domain.Users.Entity;

namespace Inventory.Domain.Users.DomainUsers;

public interface IUserGetByEmailService
{
    Task<UserEntity?> GetByEmailAsync(string email);
}
