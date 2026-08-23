using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.Actions.Entity;

namespace Inventory.Infrastructure.Persistence.Mysql.EntityConfiguration
{
    public class ActionConfiguration
    {
        public ActionConfiguration(EntityTypeBuilder<ActionEntity> builder)
        {
            // La tabla fue creada manualmente en SQL Server, EF no la gestiona en migraciones.
            builder.ToTable("Actions", t => t.ExcludeFromMigrations());

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName("ActionId");

            builder.Property(a => a.ModuleId)
                .HasColumnName("ModuleId");

            builder.Property(a => a.Code)
                .HasColumnName("Code")
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(a => a.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.IsActive)
                .HasColumnName("IsActive");

            builder.HasIndex(a => a.Code).IsUnique();

            builder.HasOne(a => a.Module)
                .WithMany()
                .HasForeignKey(a => a.ModuleId);
        }
    }
}
