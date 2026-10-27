namespace Inventory.Infrastructure.Media;

/// <summary>
/// Sección "Cloudinary" de appsettings.json, leída con el patrón Options
/// (igual que JwtOptions). ApiKey/ApiSecret se dejan vacíos en el repo —
/// se completan localmente o vía variables de entorno/user-secrets, nunca
/// se commitea un secret real.
/// </summary>
public class CloudinaryOptions
{
    public const string SectionName = "Cloudinary";

    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string? UploadPreset { get; set; }
    public int MaxImageSizeMb { get; set; } = 5;
}
