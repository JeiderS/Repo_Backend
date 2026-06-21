using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.Roles.Entity;

namespace Inventory.Infrastructure.Persistence.Mysql.EntityConfiguration
{
    public class RoleConfiguration
    {
        public RoleConfiguration(EntityTypeBuilder<RoleEntity> builder)
        {
            // La tabla fue creada manualmente en SQL Server, EF no la gestiona en migraciones.
            builder.ToTable("Roles", t => t.ExcludeFromMigrations());

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .HasColumnName("RoleId");

            builder.Property(r => r.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(r => r.Name).IsUnique();
        }
    }
}
