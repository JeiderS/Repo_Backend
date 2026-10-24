namespace Inventory.Application.Auth;

/// <summary>
/// Cuánto vive un refresh token nuevo. Un solo lugar para que la emisión
/// (login) y la rotación (refresh) nunca queden desincronizadas.
/// </summary>
public static class RefreshTokenPolicy
{
    public static readonly TimeSpan Lifetime = TimeSpan.FromDays(7);
}
