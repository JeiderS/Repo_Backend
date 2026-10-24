using System.Security.Cryptography;
using Inventory.Application.Common.Interfaces;

namespace Inventory.Infrastructure.Security;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    // 64 bytes de CSPRNG: suficiente entropía para que no haga falta un
    // "salt" por token, a diferencia de una contraseña humana (BCrypt).
    private const int RawTokenSizeInBytes = 64;

    public string GenerateRawToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(RawTokenSizeInBytes));

    public string Hash(string rawToken) =>
        Convert.ToBase64String(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken)));
}
