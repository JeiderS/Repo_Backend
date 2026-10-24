namespace Inventory.Infrastructure.Security;

/// <summary>
/// Sección "Jwt" de appsettings.json, leída con el patrón Options (igual que
/// TenantRegistryOptions). Así JwtTokenGenerator recibe valores tipados en
/// vez de buscar strings en IConfiguration.
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    // Corto a propósito: la sesión real la sostiene el refresh token
    // (httpOnly cookie, 7 días con rotación); este access token solo vive lo
    // justo para que robarlo sea poco rentable.
    public int ExpiresMinutes { get; set; } = 15;
}
