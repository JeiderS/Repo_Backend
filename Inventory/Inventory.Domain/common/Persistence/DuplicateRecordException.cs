namespace Inventory.Domain.Common.Persistence;

/// <summary>
/// Se lanza cuando la base de datos rechaza un registro por estar duplicado
/// (índice único). Es la "última defensa" para el caso raro en que dos
/// peticiones pasan la validación de unicidad al mismo tiempo.
///
/// Vive en Domain, junto a IUnitOfWork, para que nadie fuera de
/// Infrastructure tenga que conocer los códigos de error de SQL Server:
/// Infrastructure TRADUCE su excepción técnica a esta.
/// </summary>
public class DuplicateRecordException(string message, Exception innerException)
    : Exception(message, innerException);
