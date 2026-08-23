using Inventory.Domain.Users.DomainUsers;
using Inventory.Domain.Users.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Users.DomainService.Impl;

public class UserStatusService(DataBaseContext context) : IUserStatusService
{
    public async Task<UserEntity?> GetByIdAsync(int id)
    {
        return await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}
