using Inventory.Domain.UserProfile.DomainUserProfile;
using Inventory.Domain.UserProfile.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;

namespace Inventory.Infrastructure.Persistence.Mysql.UserProfile.DomainService.Impl;

public class UserProfileCreateService(DataBaseContext context) : IUserProfileCreateService
{
    public async Task AddAsync(UserProfileEntity profile)
    {
        await context.UserProfiles.AddAsync(profile);
    }
}
