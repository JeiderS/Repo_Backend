using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.Modules.Entity;

namespace Inventory.Infrastructure.Persistence.Mysql.EntityConfiguration
{
    public class ModuleConfiguration
    {
        public ModuleConfiguration(EntityTypeBuilder<ModuleEntity> builder)
        {
            // La tabla fue creada manualmente en SQL Server, EF no la gestiona en migraciones.
            builder.ToTable("Modules", t => t.ExcludeFromMigrations());

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasColumnName("ModuleId");

            builder.Property(m => m.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Icon)
                .HasColumnName("Icon")
                .HasMaxLength(100);

            builder.Property(m => m.Route)
                .HasColumnName("Route")
                .HasMaxLength(200);

            builder.Property(m => m.ParentId)
                .HasColumnName("ParentId");

            builder.Property(m => m.IsActive)
                .HasColumnName("IsActive");

            builder.Property(m => m.SortOrder)
                .HasColumnName("SortOrder");

            builder.HasIndex(m => m.Name).IsUnique();

            builder.HasOne(m => m.Parent)
                .WithMany(m => m.Children)
                .HasForeignKey(m => m.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
