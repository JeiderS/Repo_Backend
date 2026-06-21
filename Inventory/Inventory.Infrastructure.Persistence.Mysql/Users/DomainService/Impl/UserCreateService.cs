using Inventory.Application.Auth.Errors;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
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

    public async Task<Result<UserEntity, Error>> CreateAsync(UserEntity user, UserProfileEntity profile)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            profile.UserId = user.Id;
            await context.UserProfiles.AddAsync(profile);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();
            return user;
        }
        catch
        {
            await transaction.RollbackAsync();
            return AuthErrorBuilder.RegistrationException();
        }
    }
}
