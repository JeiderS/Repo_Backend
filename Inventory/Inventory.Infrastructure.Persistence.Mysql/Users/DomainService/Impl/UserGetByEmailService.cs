using Inventory.Domain.Users.DomainUsers;
using Inventory.Domain.Users.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Users.DomainService.Impl;

public class UserGetByEmailService(DataBaseContext context) : IUserGetByEmailService
{
    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        return await context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}
