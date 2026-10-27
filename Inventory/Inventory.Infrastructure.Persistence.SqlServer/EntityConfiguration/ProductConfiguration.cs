using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.Products.Entity;

namespace Inventory.Infrastructure.Persistence.SqlServer.EntityConfiguration
{
    public class ProductConfiguration
    {
        public ProductConfiguration(EntityTypeBuilder<ProductEntity> builder)
        {
            // La tabla fue creada manualmente en SQL Server, EF no la gestiona en migraciones.
            builder.ToTable("Products", t => t.ExcludeFromMigrations());

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("ProductId");

            builder.Property(p => p.ImageUrl)
                .HasColumnName("ImageUrl");

            builder.Property(p => p.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Description)
                .HasColumnName("Description")
                .HasMaxLength(500);

            builder.Property(p => p.Price)
                .HasColumnName("Price")
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.CategoryId)
                .HasColumnName("CategoryId");

            builder.Property(p => p.IsActive)
                .HasColumnName("IsActive");

            builder.Property(p => p.CreatedAt)
                .HasColumnName("CreatedAt");

            builder.Property(p => p.UpdatedAt)
                .HasColumnName("UpdatedAt");
        }
    }
}
