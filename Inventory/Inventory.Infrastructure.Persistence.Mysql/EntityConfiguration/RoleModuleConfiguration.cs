using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.RoleModules.Entity;

namespace Inventory.Infrastructure.Persistence.Mysql.EntityConfiguration
{
    public class RoleModuleConfiguration
    {
        public RoleModuleConfiguration(EntityTypeBuilder<RoleModuleEntity> builder)
        {
            // La tabla fue creada manualmente en SQL Server, EF no la gestiona en migraciones.
            builder.ToTable("RoleModules", t => t.ExcludeFromMigrations());

            builder.HasKey(rm => new { rm.RoleId, rm.ModuleId });

            builder.Property(rm => rm.RoleId).HasColumnName("RoleId");
            builder.Property(rm => rm.ModuleId).HasColumnName("ModuleId");
            builder.Property(rm => rm.CanView).HasColumnName("CanView");
            builder.Property(rm => rm.CanCreate).HasColumnName("CanCreate");
            builder.Property(rm => rm.CanEdit).HasColumnName("CanEdit");
            builder.Property(rm => rm.CanDelete).HasColumnName("CanDelete");

            builder.HasOne(rm => rm.Role)
                .WithMany()
                .HasForeignKey(rm => rm.RoleId);

            builder.HasOne(rm => rm.Module)
                .WithMany()
                .HasForeignKey(rm => rm.ModuleId);
        }
    }
}
