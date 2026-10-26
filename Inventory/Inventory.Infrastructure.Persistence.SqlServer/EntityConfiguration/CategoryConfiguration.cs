using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.Categories.Entity;

namespace Inventory.Infrastructure.Persistence.SqlServer.EntityConfiguration
{
    public class CategoryConfiguration
    {
        public CategoryConfiguration(EntityTypeBuilder<CategoryEntity> builder)
        {
            // La tabla fue creada manualmente en SQL Server, EF no la gestiona en migraciones.
            builder.ToTable("Categories", t => t.ExcludeFromMigrations());

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("CategoryId");

            builder.Property(c => c.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .HasColumnName("Description")
                .HasMaxLength(500);

            builder.Property(c => c.IsActive)
                .HasColumnName("IsActive");

            builder.Property(c => c.CreatedAt)
                .HasColumnName("CreatedAt");

            builder.Property(c => c.UpdatedAt)
                .HasColumnName("UpdatedAt");

            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
