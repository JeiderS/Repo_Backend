using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FleetManager.Domain.Drivers.Entity;

namespace FleetManager.Infrastructure.Persistence.Mysql.EntityConfiguration
{
    public class DriversConfiguration 
    {
        public DriversConfiguration(EntityTypeBuilder<DriversEntity> builder)
        {
            builder.ToTable("drivers");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.FirstName)
                .HasColumnName("first_name")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.LastName)
                .HasColumnName("last_name")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.SSN)
                .HasColumnName("ssn")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.Dob)
                .HasColumnName("dob")
                .IsRequired();

            builder.Property(d => d.Address)
                .HasColumnName("address")
                .HasMaxLength(255);

            builder.Property(d => d.City)
                .HasColumnName("city")
                .HasMaxLength(100);

            builder.Property(d => d.Zip)
                .HasColumnName("zip")
                .HasMaxLength(20);

            builder.Property(d => d.Phone)
                .HasColumnName("phone")
                .HasMaxLength(20);

            builder.Property(d => d.Active)
                .HasColumnName("active")
                .IsRequired();
        }
    }
}
