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
}
