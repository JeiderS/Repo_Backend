using Microsoft.EntityFrameworkCore;
using FleetManager.Infrastructure.Persistence.Mysql.EntityConfiguration;
using FleetManager.Domain.Drivers.Entity;
using FleetManager.Domain.Routes.Entity;
using FleetManager.Domain.Vehicles.Entity;
using FleetManager.Domain.Schedules.Entity;
using FleetManager.Domain.ScheduleView.Entity;

namespace FleetManager.Infrastructure.Persistence.Mysql.Context;
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
    }
}
