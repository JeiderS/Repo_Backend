using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Auth.Errors;

public static class AuthErrorBuilder
{
    public static Error InvalidCredentials() =>
        Error.CreateInstance(
            "AUTH_INVALID_CREDENTIALS",
            "El correo o la contraseña son incorrectos.",
            ErrorType.Unauthorized);

    public static Error InactiveUser() =>
        Error.CreateInstance(
            "AUTH_INACTIVE_USER",
            "El usuario se encuentra inactivo.",
            ErrorType.Unauthorized);

    // Deliberadamente genérico: no distingue "no existe", "expiró", "ya fue
    // usado" o "el usuario fue desactivado" — esa distinción es información
    // útil para quien intenta reusar un refresh token robado.
    public static Error InvalidRefreshToken() =>
        Error.CreateInstance(
            "AUTH_INVALID_REFRESH_TOKEN",
            "La sesión no es válida. Inicia sesión nuevamente.",
            ErrorType.Unauthorized);
}
