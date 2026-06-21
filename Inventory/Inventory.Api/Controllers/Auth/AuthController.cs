using System.Security.Claims;
using Inventory.Application.Auth.Commands.Login;
using Inventory.Application.Auth.Commands.Register;
using Inventory.Application.Auth.Queries.GetAuthorizationData;
using Inventory.Application.Common.Features;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers.Auth
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await mediator.Send(command);

            return result.Match(
                onSuccess => StatusCode(StatusCodes.Status200OK,
                    ResponseApiService.Response(StatusCodes.Status200OK, onSuccess)),
                onError => StatusCode((int)onError.HttpStatusCode,
                    ResponseApiService.Response((int)onError.HttpStatusCode, message: onError.Description)));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await mediator.Send(command);

            return result.Match(
                onSuccess => StatusCode(StatusCodes.Status200OK,
                    ResponseApiService.Response(StatusCodes.Status200OK, onSuccess)),
                onError => StatusCode((int)onError.HttpStatusCode,
                    ResponseApiService.Response((int)onError.HttpStatusCode, message: onError.Description)));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var email = User.FindFirstValue(ClaimTypes.Email);

            // Se consulta en vivo contra RoleModules: el JWT no lleva permisos,
            // así que esta es siempre la foto real y actual de la BD.
            var authorizationData = await mediator.Send(new GetAuthorizationDataQuery(userId));

            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, new
            {
                Email = email,
                Roles = authorizationData.Roles,
                Permissions = authorizationData.Permissions
            }));
        }
    }
}
