using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.Sales.Entity;

namespace Inventory.Infrastructure.Persistence.SqlServer.EntityConfiguration
{
    public class SaleConfiguration
    {
        public SaleConfiguration(EntityTypeBuilder<SaleEntity> builder)
        {
            // La tabla fue creada manualmente en SQL Server, EF no la gestiona en migraciones.
            builder.ToTable("Sales", t => t.ExcludeFromMigrations());

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .HasColumnName("SaleId");

            builder.Property(s => s.Number)
                .HasColumnName("Number")
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(s => s.SaleDate)
                .HasColumnName("SaleDate");

            builder.Property(s => s.Total)
                .HasColumnName("Total")
                .HasColumnType("decimal(18,2)");

            builder.Property(s => s.Origin)
                .HasColumnName("Origin")
                .IsRequired()
                .HasMaxLength(10);
        }
    }
}
