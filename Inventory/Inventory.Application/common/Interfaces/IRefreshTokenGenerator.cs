namespace Inventory.Application.Common.Interfaces;

public interface IRefreshTokenGenerator
{
    /// <summary>Valor crudo de alta entropía. Es lo único que ve el cliente (en la cookie).</summary>
    string GenerateRawToken();

    /// <summary>Hash del valor crudo. Es lo único que se guarda en base de datos.</summary>
    string Hash(string rawToken);
}
