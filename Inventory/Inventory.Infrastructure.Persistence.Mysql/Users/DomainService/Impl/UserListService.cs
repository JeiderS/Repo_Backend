using Inventory.Domain.Users.DomainUsers;
using Inventory.Domain.Users.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Users.DomainService.Impl;

public class UserListService(DataBaseContext context) : IUserListService
{
    public async Task<IReadOnlyList<UserEntity>> GetAllAsync()
    {
        return await context.Users
            .Include(u => u.Profile)
            .Include(u => u.Role)
            .OrderBy(u => u.Id)
            .ToListAsync();
    }
}
