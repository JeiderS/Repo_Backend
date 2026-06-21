using Microsoft.EntityFrameworkCore;
using Inventory.Infrastructure.Persistence.Mysql.EntityConfiguration;
using Inventory.Domain.Drivers.Entity;
using Inventory.Domain.Routes.Entity;
using Inventory.Domain.Vehicles.Entity;
using Inventory.Domain.Schedules.Entity;
using Inventory.Domain.ScheduleView.Entity;
using Inventory.Domain.Users.Entity;

namespace Inventory.Infrastructure.Persistence.Mysql.Context;
public class DataBaseContext : DbContext
{
    public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
    {
    }
    public DbSet<DriversEntity> Drivers { get; set; }
    public DbSet<RoutesEntity> Routes { get; set; }
    public DbSet<VehiclesEntity> Vehicles { get; set; }
    public DbSet<SchedulesEntity> Schedules { get; set; }
    public DbSet<ScheduleViewEntity> ScheduleViews { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserProfileEntity> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        EntityConfiguration(modelBuilder);
    }

    private static void EntityConfiguration(ModelBuilder modelBuilder)
    {
        new DriversConfiguration(modelBuilder.Entity<DriversEntity>());
        new RoutesConfiguration(modelBuilder.Entity<RoutesEntity>());
        new VehiclesConfiguration(modelBuilder.Entity<VehiclesEntity>());
        new SchedulesConfiguration(modelBuilder.Entity<SchedulesEntity>());
        new ScheduleViewConfiguration(modelBuilder.Entity<ScheduleViewEntity>());
        new UserConfiguration(modelBuilder.Entity<UserEntity>());
        new UserProfileConfiguration(modelBuilder.Entity<UserProfileEntity>());
    }
}
