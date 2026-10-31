using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.InventoryMovements.Entity;

namespace Inventory.Infrastructure.Persistence.SqlServer.EntityConfiguration
{
    public class InventoryMovementConfiguration
    {
        public InventoryMovementConfiguration(EntityTypeBuilder<InventoryMovementEntity> builder)
        {
            // La tabla fue creada manualmente en SQL Server, EF no la gestiona en migraciones.
            builder.ToTable("InventoryMovements", t => t.ExcludeFromMigrations());

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasColumnName("InventoryMovementId");

            builder.Property(m => m.InventoryId)
                .HasColumnName("InventoryId");

            builder.Property(m => m.MovementType)
                .HasColumnName("MovementType")
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(m => m.Quantity)
                .HasColumnName("Quantity")
                .HasColumnType("decimal(18,3)");

            builder.Property(m => m.Description)
                .HasColumnName("Description")
                .HasMaxLength(250);

            builder.Property(m => m.CreatedAt)
                .HasColumnName("CreatedAt");
        }
    }
}
