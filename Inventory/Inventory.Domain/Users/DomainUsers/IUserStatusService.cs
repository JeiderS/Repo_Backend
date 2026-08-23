using Inventory.Domain.Users.Entity;

namespace Inventory.Domain.Users.DomainUsers;

public interface IUserStatusService
{
    /// <summary>
    /// Fetch scoped to the status use-case: no Profile include, unlike
    /// IUserGetByIdService (used by the edit flow, which needs profile fields).
    /// </summary>
    Task<UserEntity?> GetByIdAsync(int id);
}
