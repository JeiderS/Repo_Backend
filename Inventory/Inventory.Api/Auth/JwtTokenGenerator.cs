using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Users.Entity;
using Microsoft.IdentityModel.Tokens;

namespace Inventory.Api.Auth;

public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    // El JWT solo lleva identidad + roles. Los permisos se resuelven contra
    // RoleModules en cada request (ver HasPermissionAttribute), nunca aquí.
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
            new(ClaimTypes.Name, string.IsNullOrEmpty(fullName) ? user.Email : fullName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

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
