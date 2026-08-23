using Inventory.Domain.Users.Entity;

namespace Inventory.Domain.Users.DomainUsers;

public interface IUserListService
{
    Task<IReadOnlyList<UserEntity>> GetAllAsync();
}
