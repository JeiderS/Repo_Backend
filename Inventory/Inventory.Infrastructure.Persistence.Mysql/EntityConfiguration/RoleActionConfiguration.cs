using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.RoleActions.Entity;

namespace Inventory.Infrastructure.Persistence.Mysql.EntityConfiguration
{
    public class RoleActionConfiguration
    {
        public RoleActionConfiguration(EntityTypeBuilder<RoleActionEntity> builder)
        {
            // La tabla fue creada manualmente en SQL Server, EF no la gestiona en migraciones.
            builder.ToTable("RoleActions", t => t.ExcludeFromMigrations());

            builder.HasKey(ra => new { ra.RoleId, ra.ActionId });

            builder.Property(ra => ra.RoleId).HasColumnName("RoleId");
            builder.Property(ra => ra.ActionId).HasColumnName("ActionId");

            builder.HasOne(ra => ra.Role)
                .WithMany()
                .HasForeignKey(ra => ra.RoleId);

            builder.HasOne(ra => ra.Action)
                .WithMany()
                .HasForeignKey(ra => ra.ActionId);
        }
    }
}
