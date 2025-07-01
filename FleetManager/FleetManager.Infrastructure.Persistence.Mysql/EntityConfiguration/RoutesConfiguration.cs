using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FleetManager.Domain.Routes.Entity;

namespace FleetManager.Infrastructure.Persistence.Mysql.EntityConfiguration
{
    public class RoutesConfiguration 
    {
        public  RoutesConfiguration(EntityTypeBuilder<RoutesEntity> builder)
        {
            builder.ToTable("routes");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Description)
                .HasColumnName("description")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.DriverId)
                .HasColumnName("driver_id")
                .IsRequired();

            builder.Property(r => r.VehicleId)
                .HasColumnName("vehicle_id")
                .IsRequired();

            builder.Property(r => r.Active)
                .HasColumnName("active")
                .IsRequired();
        }
    }
}
