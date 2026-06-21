using Inventory.Domain.Users.Entity;

namespace Inventory.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    /// <summary>
    /// El JWT solo lleva identidad (Id, Email, Nombre) y roles. Los permisos
    /// NUNCA se incluyen aquí: se resuelven contra la BD en cada request
    /// protegido para que un cambio de permisos sea efectivo de inmediato.
    /// </summary>
    string GenerateToken(UserEntity user, IReadOnlyList<string> roles);
}
