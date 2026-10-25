using Inventory.Domain.RefreshTokens.Entity;
using Inventory.Domain.RefreshTokens.Repositories;
using Inventory.Infrastructure.Persistence.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.SqlServer.Repositories;

public class RefreshTokenRepository(DataBaseContext context) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshTokenEntity token)
    {
        await context.RefreshTokens.AddAsync(token);
    }

    public async Task<RefreshTokenEntity?> GetByTokenHashAsync(string tokenHash)
    {
        return await context.RefreshTokens
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash);
    }
}
