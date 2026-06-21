using Inventory.Domain.Users.DomainUsers;
using Inventory.Domain.Users.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Users.DomainService.Impl;

public class UserCreateService(DataBaseContext context) : IUserCreateService
{
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task AddAsync(UserEntity user)
    {
        await context.Users.AddAsync(user);
    }
}
