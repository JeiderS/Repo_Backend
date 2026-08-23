using Inventory.Domain.Users.Entity;

namespace Inventory.Domain.Users.DomainUsers;

public interface IUserGetByIdService
{
    Task<UserEntity?> GetByIdAsync(int id);
}
