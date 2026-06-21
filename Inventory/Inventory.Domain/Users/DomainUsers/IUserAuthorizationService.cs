using Inventory.Domain.Common.Authorization;

namespace Inventory.Domain.Users.DomainUsers;

public interface IUserAuthorizationService
{
    /// <summary>
    /// Solo los nombres de rol. Es lo único que debe viajar dentro del JWT.
    /// </summary>
    Task<IReadOnlyList<string>> GetRolesAsync(int userId);

    /// <summary>
    /// Foto completa de roles + permisos por módulo. Se calcula contra la BD
    /// en el momento (no se cachea en el token) para que cualquier cambio de
    /// permisos sea efectivo de inmediato. Pensado para endpoints de consulta
    /// como /auth/me, no para validarse en cada request individual.
    /// </summary>
    Task<UserAuthorizationData> GetAuthorizationDataAsync(int userId);

    /// <summary>
    /// Verificación puntual y barata de un permiso concreto contra la BD.
    /// Es la que debe usar cualquier filtro de autorización por petición.
    /// </summary>
    Task<bool> HasPermissionAsync(int userId, string module, string action);
}
