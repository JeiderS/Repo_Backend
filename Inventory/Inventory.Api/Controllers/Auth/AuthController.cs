using System.Security.Claims;
using Inventory.Api.Controllers.Common;
using Inventory.Application.Auth;
using Inventory.Application.Auth.Commands.Login;
using Inventory.Application.Auth.Commands.Logout;
using Inventory.Application.Auth.Commands.RefreshToken;
using Inventory.Application.Auth.Queries.GetAuthorizationData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Inventory.Api.Controllers.Auth
{
    [Route("api/v1/auth")]
    public class AuthController(IMediator mediator) : ApiControllerBase
    {
        // Mandar la cookie en cada request de la API sería desperdicio (y
        // superficie extra); solo hace falta que viaje hacia/desde estos
        // tres endpoints de auth.
        private const string RefreshTokenCookieName = "refreshToken";
        private const string AuthCookiePath = "/api/v1/auth";

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [EnableRateLimiting("login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await mediator.Send(command);

            if (result.IsSuccess)
                SetRefreshTokenCookie(result.Value!.RefreshToken);

            return FromResult(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var rawToken) || string.IsNullOrEmpty(rawToken))
                return Unauthorized();

            var result = await mediator.Send(new RefreshTokenCommand { RawToken = rawToken });

            if (result.IsSuccess)
                SetRefreshTokenCookie(result.Value!.RefreshToken);

            return FromResult(result);
        }

        // [AllowAnonymous] a propósito: un usuario cuyo access token ya
        // expiró (lo normal, dura 15 min) debe poder seguir cerrando sesión.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            Request.Cookies.TryGetValue(RefreshTokenCookieName, out var rawToken);

            await mediator.Send(new LogoutCommand { RawToken = rawToken });

            ClearRefreshTokenCookie();

            return Success(null);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var email = User.FindFirstValue(ClaimTypes.Email);

            // Se consulta en vivo contra RoleActions: el JWT no lleva permisos,
            // así que esta es siempre la foto real y actual de la BD.
            var authorizationData = await mediator.Send(new GetAuthorizationDataQuery(userId));

            return Success(new
            {
                Email = email,
                Roles = authorizationData.Roles,
                Permissions = authorizationData.Permissions
            });
        }

        private void SetRefreshTokenCookie(string rawToken)
        {
            Response.Cookies.Append(RefreshTokenCookieName, rawToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = AuthCookiePath,
                Expires = DateTimeOffset.UtcNow.Add(RefreshTokenPolicy.Lifetime)
            });
        }

        private void ClearRefreshTokenCookie()
        {
            Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions { Path = AuthCookiePath });
        }
    }
}
