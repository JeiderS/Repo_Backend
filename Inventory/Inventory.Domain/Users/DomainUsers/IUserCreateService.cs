using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Users.Entity;

namespace Inventory.Domain.Users.DomainUsers;

public interface IUserCreateService
{
    Task<bool> EmailExistsAsync(string email);
    Task<Result<UserEntity, Error>> CreateAsync(UserEntity user, UserProfileEntity profile);
}
