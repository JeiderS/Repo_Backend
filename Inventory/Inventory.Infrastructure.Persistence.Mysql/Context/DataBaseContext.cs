using Microsoft.EntityFrameworkCore;
using Inventory.Infrastructure.Persistence.Mysql.EntityConfiguration;
using Inventory.Domain.Users.Entity;
using Inventory.Domain.UserProfile.Entity;
using Inventory.Domain.Roles.Entity;
using Inventory.Domain.Modules.Entity;
using Inventory.Domain.Actions.Entity;
using Inventory.Domain.RoleActions.Entity;

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
    public DbSet<ActionEntity> Actions { get; set; }
    public DbSet<RoleActionEntity> RoleActions { get; set; }

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
        new ActionConfiguration(modelBuilder.Entity<ActionEntity>());
        new RoleActionConfiguration(modelBuilder.Entity<RoleActionEntity>());

        // UserRoles (N:M legacy) ya no se mapea desde Checkpoint B: el usuario
        // tiene un unico Users.RoleId (ver UserConfiguration). La tabla en si
        // sigue existiendo en SQL Server hasta Checkpoint C (03_Drop_UserRoles.sql),
        // pero EF ya no la conoce.
        //
        // RoleModules (Checkpoint B2, esta misma release) tampoco se mapea
        // más: RoleActions es la única fuente de permisos enforced (design.md,
        // action-code-authorization). La tabla RoleModules sigue existiendo en
        // SQL Server hasta Checkpoint C (02_Drop_RoleModules.sql, gated, per
        // tenant), pero EF ya no la conoce.
    }
}
