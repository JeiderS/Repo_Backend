using Inventory.Domain.Users.DomainUsers;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Users.DomainService.Impl;

public class UserUpdateService(DataBaseContext context) : IUserUpdateService
{
    public async Task<bool> EmailExistsForOtherAsync(string email, int excludeId)
    {
        return await context.Users.AnyAsync(u => u.Email == email && u.Id != excludeId);
    }
}
