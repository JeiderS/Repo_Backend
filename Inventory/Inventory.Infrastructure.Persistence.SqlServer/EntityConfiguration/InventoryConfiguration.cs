using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.Inventories.Entity;

namespace Inventory.Infrastructure.Persistence.SqlServer.EntityConfiguration
{
    public class InventoryConfiguration
    {
        public InventoryConfiguration(EntityTypeBuilder<InventoryEntity> builder)
        {
            // La tabla fue creada manualmente en SQL Server, EF no la gestiona en migraciones.
            builder.ToTable("Inventories", t => t.ExcludeFromMigrations());

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                .HasColumnName("InventoryId");

            builder.Property(i => i.ProductId)
                .HasColumnName("ProductId");

            builder.Property(i => i.Quantity)
                .HasColumnName("Quantity")
                .HasColumnType("decimal(18,3)");

            builder.Property(i => i.UpdatedAt)
                .HasColumnName("UpdatedAt");

            builder.HasIndex(i => i.ProductId)
                .IsUnique();
        }
    }
}
