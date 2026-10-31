using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.SaleDetails.Entity;

namespace Inventory.Infrastructure.Persistence.SqlServer.EntityConfiguration
{
    public class SaleDetailConfiguration
    {
        public SaleDetailConfiguration(EntityTypeBuilder<SaleDetailEntity> builder)
        {
            // La tabla fue creada manualmente en SQL Server, EF no la gestiona en migraciones.
            builder.ToTable("SaleDetails", t => t.ExcludeFromMigrations());

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .HasColumnName("SaleDetailId");

            builder.Property(d => d.SaleId)
                .HasColumnName("SaleId");

            builder.Property(d => d.ProductId)
                .HasColumnName("ProductId");

            builder.Property(d => d.Quantity)
                .HasColumnName("Quantity")
                .HasColumnType("decimal(18,3)");

            builder.Property(d => d.UnitPrice)
                .HasColumnName("UnitPrice")
                .HasColumnType("decimal(18,2)");

            builder.Property(d => d.Subtotal)
                .HasColumnName("Subtotal")
                .HasColumnType("decimal(18,2)");
        }
    }
}
