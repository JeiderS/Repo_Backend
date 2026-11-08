using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.Customers.Entity;

namespace Inventory.Infrastructure.Persistence.SqlServer.EntityConfiguration
{
    public class CustomerConfiguration
    {
        public CustomerConfiguration(EntityTypeBuilder<CustomerEntity> builder)
        {
            // La tabla fue creada manualmente en SQL Server, EF no la gestiona en migraciones.
            builder.ToTable("Customers", t => t.ExcludeFromMigrations());

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("CustomerId");

            builder.Property(c => c.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Phone)
                .HasColumnName("Phone")
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(c => c.Email)
                .HasColumnName("Email")
                .HasMaxLength(150);

            builder.Property(c => c.Address)
                .HasColumnName("Address")
                .HasMaxLength(250);

            builder.Property(c => c.IsActive)
                .HasColumnName("IsActive");

            builder.Property(c => c.CreatedAt)
                .HasColumnName("CreatedAt");

            builder.Property(c => c.UpdatedAt)
                .HasColumnName("UpdatedAt");
        }
    }
}
