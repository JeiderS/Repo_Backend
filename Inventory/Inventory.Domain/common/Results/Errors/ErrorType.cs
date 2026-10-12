namespace Inventory.Domain.Common.Results.Errors;

/// <summary>
/// Qué TIPO de error ocurrió, dicho en lenguaje de negocio.
/// El dominio no sabe nada de HTTP: es la capa Api la que traduce cada
/// tipo a un código de estado (NotFound -> 404, Conflict -> 409, ...).
/// Así el mismo error sirve igual desde una API, una consola o una cola.
/// </summary>
public enum ErrorType
{
    /// <summary>Los datos recibidos no cumplen una regla.</summary>
    Validation,

    /// <summary>Lo que se busca no existe.</summary>
    NotFound,

    /// <summary>Choca con algo que ya existe (ej. email o nombre repetido).</summary>
    Conflict,

    /// <summary>No se pudo identificar a quien hace la petición.</summary>
    Unauthorized,

    /// <summary>Falla inesperada al completar la operación.</summary>
    Failure
}
