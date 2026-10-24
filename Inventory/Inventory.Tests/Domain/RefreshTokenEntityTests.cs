using Inventory.Domain.RefreshTokens.Entity;
using Xunit;

namespace Inventory.Tests.Domain;

/// <summary>
/// Pruebas de las reglas de negocio del refresh token. No necesitan base de
/// datos: la entidad decide sola si está utilizable.
/// </summary>
public class RefreshTokenEntityTests
{
    [Fact]
    public void Create_NewToken_IsActive()
    {
        var nowUtc = DateTime.UtcNow;
        var token = RefreshTokenEntity.Create(1, "hash", nowUtc, nowUtc.AddDays(7));

        Assert.True(token.IsActive(nowUtc));
    }

    [Fact]
    public void Revoke_MakesTokenInactive_EvenBeforeItExpires()
    {
        var nowUtc = DateTime.UtcNow;
        var token = RefreshTokenEntity.Create(1, "hash", nowUtc, nowUtc.AddDays(7));

        token.Revoke(nowUtc);

        Assert.False(token.IsActive(nowUtc));
    }

    [Fact]
    public void IsActive_AfterExpiration_ReturnsFalse()
    {
        var nowUtc = DateTime.UtcNow;
        var token = RefreshTokenEntity.Create(1, "hash", nowUtc, nowUtc.AddDays(7));

        Assert.False(token.IsActive(nowUtc.AddDays(7).AddSeconds(1)));
    }

    [Fact]
    public void Revoke_CalledTwice_KeepsTheFirstRevocationTime()
    {
        var nowUtc = DateTime.UtcNow;
        var token = RefreshTokenEntity.Create(1, "hash", nowUtc, nowUtc.AddDays(7));

        token.Revoke(nowUtc);
        token.Revoke(nowUtc.AddMinutes(5));

        Assert.False(token.IsActive(nowUtc));
    }
}
