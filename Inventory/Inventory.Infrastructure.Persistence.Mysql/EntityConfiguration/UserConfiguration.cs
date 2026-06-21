using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.Users.Entity;

namespace Inventory.Infrastructure.Persistence.Mysql.EntityConfiguration
{
    public class UserConfiguration
    {
        public UserConfiguration(EntityTypeBuilder<UserEntity> builder)
        {
            // La tabla fue creada manualmente en SQL Server, EF no la gestiona en migraciones.
            builder.ToTable("Users", t => t.ExcludeFromMigrations());

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName("UserId");

            builder.Property(u => u.Email)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.PasswordHash)
                .HasColumnName("PasswordHash")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.IsActive)
                .HasColumnName("IsActive");

            builder.Property(u => u.CreatedAt)
                .HasColumnName("CreatedAt");

            builder.HasIndex(u => u.Email).IsUnique();

            builder.HasOne(u => u.Profile)
                .WithOne()
                .HasForeignKey<UserProfileEntity>(p => p.UserId);
        }
    }
}
