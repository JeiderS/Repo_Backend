using System.Security.Claims;
using Inventory.Api.Controllers.Common;
using Inventory.Application.Auth.Commands.Login;
using Inventory.Application.Auth.Queries.GetAuthorizationData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers.Auth
{
    [Route("api/v1/auth")]
    public class AuthController(IMediator mediator) : ApiControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await mediator.Send(command);
            return FromResult(result);
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
    }
}
