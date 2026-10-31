using Inventory.Domain.Actions.Entity;
using Inventory.Domain.Categories.Entity;
using Inventory.Domain.Inventories.Entity;
using Inventory.Domain.InventoryMovements.Entity;
using Inventory.Domain.ModuleActions.Entity;
using Inventory.Domain.Modules.Entity;
using Inventory.Domain.Products.Entity;
using Inventory.Domain.RefreshTokens.Entity;
using Inventory.Domain.RoleActions.Entity;
using Inventory.Domain.Roles.Entity;
using Inventory.Domain.SaleDetails.Entity;
using Inventory.Domain.Sales.Entity;
using Inventory.Domain.UserProfile.Entity;
using Inventory.Domain.Users.Entity;
using Inventory.Infrastructure.Persistence.SqlServer.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.SqlServer.Context;
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
    public DbSet<ModuleActionEntity> ModuleActions { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<InventoryEntity> Inventories { get; set; }
    public DbSet<InventoryMovementEntity> InventoryMovements { get; set; }
    public DbSet<SaleEntity> Sales { get; set; }
    public DbSet<SaleDetailEntity> SaleDetails { get; set; }

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
        new ModuleActionConfiguration(modelBuilder.Entity<ModuleActionEntity>());
        new RefreshTokenConfiguration(modelBuilder.Entity<RefreshTokenEntity>());
        new CategoryConfiguration(modelBuilder.Entity<CategoryEntity>());
        new ProductConfiguration(modelBuilder.Entity<ProductEntity>());
        new InventoryConfiguration(modelBuilder.Entity<InventoryEntity>());
        new InventoryMovementConfiguration(modelBuilder.Entity<InventoryMovementEntity>());
        new SaleConfiguration(modelBuilder.Entity<SaleEntity>());
        new SaleDetailConfiguration(modelBuilder.Entity<SaleDetailEntity>());
    }
}
