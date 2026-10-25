using Inventory.Domain.ModuleActions.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.SqlServer.EntityConfiguration
{
    public class ModuleActionConfiguration
    {
        public ModuleActionConfiguration(
            EntityTypeBuilder<ModuleActionEntity> builder)
        {
            builder.ToTable(
                "ModuleActions",
                t => t.ExcludeFromMigrations());

            builder.HasKey(ma => new
            {
                ma.ModuleId,
                ma.ActionId
            });

            builder.Property(ma => ma.ModuleId)
                .HasColumnName("ModuleId");

            builder.Property(ma => ma.ActionId)
                .HasColumnName("ActionId");

            builder.HasOne(ma => ma.Module)
                .WithMany()
                .HasForeignKey(ma => ma.ModuleId);

            builder.HasOne(ma => ma.Action)
                .WithMany(a => a.ModuleActions)
                .HasForeignKey(ma => ma.ActionId);
        }
    }
}