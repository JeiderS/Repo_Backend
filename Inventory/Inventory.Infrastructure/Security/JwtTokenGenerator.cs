using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Users.Entity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Inventory.Infrastructure.Security;

public class JwtTokenGenerator(IOptions<JwtOptions> options, ITenantContext tenantContext) : IJwtTokenGenerator
{
    // El JWT solo lleva identidad, nunca nombres de rol ni permisos.
    // PermissionClaimsMiddleware deriva los claims de Action code (y
    // system_admin) en cada request contra RoleActions/Roles — el JWT no es
    // fuente de autorización (design.md D2, spec: action-code-authorization
    // "Role-Name Claims Removed from Issued Tokens"). El parámetro roles se
    // conserva sin uso para no cambiar la firma de IJwtTokenGenerator.
    public string GenerateToken(UserEntity user, IReadOnlyList<string> roles)
    {
        var jwt = options.Value;

        // La regla de cómo se arma el nombre completo vive en la entidad.
        var fullName = user.FullName;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, string.IsNullOrEmpty(fullName) ? user.Email : fullName),
            // Defense-in-depth: enforced by TenantClaimValidationMiddleware on
            // every subsequent authenticated request (spec: tenant-scoped-login,
            // JWT Tenant Claim requirement).
            new("tenant", tenantContext.Key)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwt.ExpiresMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
