using Inventory.Application.Common.Features;
using Inventory.Application.Users.Command.CreateUser;
using Inventory.Application.Users.Command.SetUserStatus;
using Inventory.Application.Users.Command.UpdateUser;
using Inventory.Application.Users.Query.GetUserById;
using Inventory.Application.Users.Query.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers.Users
{
    [ApiController]
    [Route("api/v1/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await mediator.Send(new GetUsersQuery());

            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, users));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await mediator.Send(new GetUserByIdQuery(id));

            if (user is null)
                return StatusCode(StatusCodes.Status404NotFound,
                    ResponseApiService.Response(StatusCodes.Status404NotFound, message: "El usuario indicado no existe."));

            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, user));
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var createdUser = await mediator.Send(command);

            return createdUser.Match(
                onSuccess => StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, onSuccess)),
                onError => StatusCode((int)onError.HttpStatusCode, ResponseApiService.Response((int)onError.HttpStatusCode, message: onError.Description)));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;
            var updatedUser = await mediator.Send(command);

            return updatedUser.Match(
                onSuccess => StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, onSuccess)),
                onError => StatusCode((int)onError.HttpStatusCode, ResponseApiService.Response((int)onError.HttpStatusCode, message: onError.Description)));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> SetStatus(int id, [FromBody] SetUserStatusCommand command)
        {
            command.Id = id;
            var updatedUser = await mediator.Send(command);

            return updatedUser.Match(
                onSuccess => StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, onSuccess)),
                onError => StatusCode((int)onError.HttpStatusCode, ResponseApiService.Response((int)onError.HttpStatusCode, message: onError.Description)));
        }
    }
}
