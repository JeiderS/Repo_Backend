using Microsoft.EntityFrameworkCore;
using Inventory.Infrastructure.Persistence.Mysql.EntityConfiguration;
using Inventory.Domain.Users.Entity;
using Inventory.Domain.UserProfile.Entity;

namespace Inventory.Infrastructure.Persistence.Mysql.Context;
public class DataBaseContext : DbContext
{
    public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
    {
    }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserProfileEntity> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        EntityConfiguration(modelBuilder);
    }

    private static void EntityConfiguration(ModelBuilder modelBuilder)
    {
        new UserConfiguration(modelBuilder.Entity<UserEntity>());
        new UserProfileConfiguration(modelBuilder.Entity<UserProfileEntity>());
    }
}
