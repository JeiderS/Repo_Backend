using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Domain.RefreshTokens.Entity;

namespace Inventory.Infrastructure.Persistence.Mysql.EntityConfiguration
{
    public class RefreshTokenConfiguration
    {
        public RefreshTokenConfiguration(EntityTypeBuilder<RefreshTokenEntity> builder)
        {
            // Misma convención que el resto de las tablas de este proyecto:
            // DDL manual en Sql/Migrations, EF no la gestiona.
            builder.ToTable("RefreshTokens", t => t.ExcludeFromMigrations());

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .HasColumnName("RefreshTokenId");

            builder.Property(r => r.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            builder.Property(r => r.TokenHash)
                .HasColumnName("TokenHash")
                .IsRequired()
                .HasMaxLength(100); // Base64 de SHA-256 (32 bytes) = 44 caracteres.

            builder.Property(r => r.CreatedAtUtc)
                .HasColumnName("CreatedAt")
                .IsRequired();

            builder.Property(r => r.ExpiresAtUtc)
                .HasColumnName("ExpiresAt")
                .IsRequired();

            // Concurrency token: si dos requests intentan rotar el mismo
            // token a la vez, el segundo UPDATE pierde (DbUpdateConcurrencyException)
            // en vez de rotar igual sobre un valor ya revocado por el primero.
            builder.Property(r => r.RevokedAtUtc)
                .HasColumnName("RevokedAt")
                .IsConcurrencyToken();

            // Para que GetByTokenHashAsync (una lookup por request de refresh)
            // no haga table scan.
            builder.HasIndex(r => r.TokenHash).IsUnique();

            builder.HasOne<Inventory.Domain.Users.Entity.UserEntity>()
                .WithMany()
                .HasForeignKey(r => r.UserId);
        }
    }
}
