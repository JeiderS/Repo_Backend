using Inventory.Domain.Users.Entity;

namespace Inventory.Domain.Users.DomainUsers;

public interface IUserCreateService
{
    Task<bool> EmailExistsAsync(string email);
    Task AddAsync(UserEntity user);
}
