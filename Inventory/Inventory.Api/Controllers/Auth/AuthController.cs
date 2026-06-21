using Inventory.Application.Auth.Commands.Login;
using Inventory.Application.Auth.Commands.Register;
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
    }
}
