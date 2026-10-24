using Inventory.Domain.RefreshTokens.Entity;

namespace Inventory.Domain.RefreshTokens.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshTokenEntity token);

    /// <summary>El token con ese hash, o null si no existe.</summary>
    Task<RefreshTokenEntity?> GetByTokenHashAsync(string tokenHash);
}
