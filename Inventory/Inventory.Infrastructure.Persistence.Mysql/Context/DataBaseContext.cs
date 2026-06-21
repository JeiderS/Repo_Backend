using Microsoft.EntityFrameworkCore;
using Inventory.Infrastructure.Persistence.Mysql.EntityConfiguration;
using Inventory.Domain.Users.Entity;
using Inventory.Domain.UserProfile.Entity;
using Inventory.Domain.Roles.Entity;
using Inventory.Domain.Modules.Entity;
using Inventory.Domain.RoleModules.Entity;

namespace Inventory.Infrastructure.Persistence.Mysql.Context;
public class DataBaseContext : DbContext
{
    public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
    {
    }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserProfileEntity> UserProfiles { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<ModuleEntity> Modules { get; set; }
    public DbSet<RoleModuleEntity> RoleModules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        EntityConfiguration(modelBuilder);
    }

    private static void EntityConfiguration(ModelBuilder modelBuilder)
    {
        new UserConfiguration(modelBuilder.Entity<UserEntity>());
        new UserProfileConfiguration(modelBuilder.Entity<UserProfileEntity>());
        new RoleConfiguration(modelBuilder.Entity<RoleEntity>());
        new ModuleConfiguration(modelBuilder.Entity<ModuleEntity>());
        new RoleModuleConfiguration(modelBuilder.Entity<RoleModuleEntity>());

        // Tabla puente UserRoles creada manualmente en SQL Server (UserId, RoleId).
        modelBuilder.Entity<UserEntity>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserRoles",
                j => j.HasOne<RoleEntity>().WithMany().HasForeignKey("RoleId"),
                j => j.HasOne<UserEntity>().WithMany().HasForeignKey("UserId"),
                j =>
                {
                    j.ToTable("UserRoles", t => t.ExcludeFromMigrations());
                    j.Property<int>("UserId");
                    j.Property<int>("RoleId");
                    j.HasKey("UserId", "RoleId");
                });
    }
}
