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
    public int ExpiresMinutes { get; set; } = 120;
}
