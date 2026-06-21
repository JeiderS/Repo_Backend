using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.Vehicles.Entity;

namespace Inventory.Infrastructure.Persistence.Mysql.EntityConfiguration
{
    public class VehiclesConfiguration 
    {
        public VehiclesConfiguration(EntityTypeBuilder<VehiclesEntity> builder)
        {
            builder.ToTable("vehicles");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Description)
                .HasColumnName("description")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(v => v.Year)
                .HasColumnName("year")
                .IsRequired();

            builder.Property(v => v.Make)
                .HasColumnName("make")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Capacity)
                .HasColumnName("capacity")
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(v => v.Active)
                .HasColumnName("active")
                .IsRequired();
        }
    }
}
