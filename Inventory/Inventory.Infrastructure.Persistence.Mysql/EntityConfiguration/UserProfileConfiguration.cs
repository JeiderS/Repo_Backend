using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.UserProfile.Entity;

namespace Inventory.Infrastructure.Persistence.Mysql.EntityConfiguration
{
    public class UserProfileConfiguration
    {
        public UserProfileConfiguration(EntityTypeBuilder<UserProfileEntity> builder)
        {
            // La tabla fue creada manualmente en SQL Server, EF no la gestiona en migraciones.
            builder.ToTable("UserProfile", t => t.ExcludeFromMigrations());

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("ProfileId");

            builder.Property(p => p.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            builder.Property(p => p.FirstName)
                .HasColumnName("FirstName")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.LastName)
                .HasColumnName("LastName")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.PhotoUrl)
                .HasColumnName("PhotoUrl")
                .HasMaxLength(500);

            builder.Property(p => p.Phone)
                .HasColumnName("Phone")
                .HasMaxLength(30);

            builder.Property(p => p.Address)
                .HasColumnName("Address")
                .HasMaxLength(255);
        }
    }
}
