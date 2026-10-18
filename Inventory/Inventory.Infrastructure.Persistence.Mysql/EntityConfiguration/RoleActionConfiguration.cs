using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.RoleActions.Entity;

namespace Inventory.Infrastructure.Persistence.Mysql.EntityConfiguration
{
    public class RoleActionConfiguration
    {
        public RoleActionConfiguration(
            EntityTypeBuilder<RoleActionEntity> builder)
        {
            builder.ToTable(
                "RoleActions",
                t => t.ExcludeFromMigrations());

            builder.HasKey(ra => new
            {
                ra.RoleId,
                ra.ModuleId,
                ra.ActionId
            });

            builder.Property(ra => ra.RoleId)
                .HasColumnName("RoleId");

            builder.Property(ra => ra.ModuleId)
                .HasColumnName("ModuleId");

            builder.Property(ra => ra.ActionId)
                .HasColumnName("ActionId");

            builder.HasOne(ra => ra.Role)
                .WithMany()
                .HasForeignKey(ra => ra.RoleId);

            builder.HasOne(ra => ra.ModuleAction)
                .WithMany()
                .HasForeignKey(ra => new
                {
                    ra.ModuleId,
                    ra.ActionId
                });
        }
    }
}
