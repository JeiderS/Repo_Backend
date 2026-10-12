using Inventory.Domain.Users.Entity;
using Inventory.Domain.Users.Repositories;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Mysql.Repositories;

/// <summary>
/// Implementa IUserRepository con EF Core. Solo trae y guarda datos:
/// no toma decisiones de negocio.
/// </summary>
public class UserRepository(DataBaseContext context) : IUserRepository
{
    public async Task<UserEntity?> GetByIdAsync(int id)
    {
        return await context.Users
            .Include(u => u.Profile)
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        return await context.Users
            .Include(u => u.Profile)
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IReadOnlyList<UserEntity>> GetAllAsync()
    {
        return await context.Users
            .Include(u => u.Profile)
            .Include(u => u.Role)
            .OrderBy(u => u.Id)
            .ToListAsync();
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
    {
        return await context.Users
            .AnyAsync(u => u.Email == email && (excludeUserId == null || u.Id != excludeUserId));
    }

    public async Task AddAsync(UserEntity user)
    {
        // EF Core agrega todo el "grafo": si user.Profile está asignado, el
        // perfil también se inserta. Por eso ya no hace falta un repositorio
        // aparte para UserProfile.
        await context.Users.AddAsync(user);
    }

    public async Task<bool> IsActiveAsync(int userId)
    {
        return await context.Users
            .Where(u => u.Id == userId)
            .Select(u => (bool?)u.IsActive)
            .FirstOrDefaultAsync() ?? false;
    }

    public async Task<int?> GetRoleIdAsync(int userId)
    {
        return await context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.RoleId)
            .FirstOrDefaultAsync();
    }
}
