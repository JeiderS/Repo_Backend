using Inventory.Domain.UserProfile.Entity;

namespace Inventory.Domain.UserProfile.DomainUserProfile;

public interface IUserProfileCreateService
{
    Task AddAsync(UserProfileEntity profile);
}
