using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Users.Entity;
using Microsoft.IdentityModel.Tokens;

namespace Inventory.Api.Auth;

public class JwtTokenGenerator(IConfiguration configuration, ITenantContext tenantContext) : IJwtTokenGenerator
{
    // El JWT solo lleva identidad, nunca nombres de rol ni permisos. Desde
    // Checkpoint B, PermissionClaimsMiddleware deriva los claims de Action
    // code (y system_admin) en cada request contra RoleActions/Roles — el JWT
    // no es fuente de autorización (design.md D2, spec: action-code-authorization
    // "Role-Name Claims Removed from Issued Tokens"). El parámetro roles se
    // conserva sin uso: la firma de IJwtTokenGenerator y LoginCommandHandler
    // no cambian (design.md File Changes).
    public string GenerateToken(UserEntity user, IReadOnlyList<string> roles)
    {
        var key = configuration["Jwt:Key"]!;
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var expiresMinutes = int.TryParse(configuration["Jwt:ExpiresMinutes"], out var m) ? m : 120;

        var fullName = user.Profile is null
            ? string.Empty
            : $"{user.Profile.FirstName} {user.Profile.LastName}".Trim();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, string.IsNullOrEmpty(fullName) ? user.Email : fullName),
            // Defense-in-depth: enforced by TenantClaimValidationMiddleware on
            // every subsequent authenticated request (spec: tenant-scoped-login,
            // JWT Tenant Claim requirement). Ships atomically with that
            // middleware — see Program.cs Checkpoint B.
            new("tenant", tenantContext.Key)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
