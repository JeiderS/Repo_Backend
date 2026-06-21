using System.Net;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Application.Auth.Errors;

public static class AuthErrorBuilder
{
    public static Error InvalidCredentials() =>
        Error.CreateInstance(
            "AUTH_INVALID_CREDENTIALS",
            "El correo o la contraseña son incorrectos.",
            HttpStatusCode.Unauthorized);

    public static Error InactiveUser() =>
        Error.CreateInstance(
            "AUTH_INACTIVE_USER",
            "El usuario se encuentra inactivo.",
            HttpStatusCode.Unauthorized);

    public static Error EmailAlreadyExists() =>
        Error.CreateInstance(
            "AUTH_EMAIL_EXISTS",
            "El correo ya se encuentra registrado.",
            HttpStatusCode.Conflict);

    public static Error RegistrationException() =>
        Error.CreateInstance(
            "AUTH_REGISTRATION_ERROR",
            "No se pudo registrar el usuario.",
            HttpStatusCode.InternalServerError);
}
