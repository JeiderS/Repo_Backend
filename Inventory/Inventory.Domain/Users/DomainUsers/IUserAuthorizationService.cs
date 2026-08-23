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
    /// <remarks>
    /// HasPermissionAsync fue eliminado en Checkpoint B: su único caller era
    /// HasPermissionAttribute, ya eliminado (dead desde Phase 1). La
    /// autorización por request ahora pasa por PermissionClaimsMiddleware +
    /// [Authorize], no por un filtro que consulta BD por acción.
    /// </remarks>
    Task<UserAuthorizationData> GetAuthorizationDataAsync(int userId);
}
