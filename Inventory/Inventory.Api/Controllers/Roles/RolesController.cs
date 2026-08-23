using Inventory.Application.Common.Features;
using Inventory.Application.Roles.Command.AssignRoleActions;
using Inventory.Application.Roles.Command.CreateRole;
using Inventory.Application.Roles.Command.UpdateRole;
using Inventory.Application.Roles.Query.GetRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers.Roles
{
    [ApiController]
    [Route("api/v1/roles")]
    public class RolesController(IMediator mediator) : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "RolesView")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await mediator.Send(new GetRolesQuery());

            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, roles));
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "RolesCreate")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
        {
            var createdRole = await mediator.Send(command);

            return createdRole.Match(
                onSuccess => StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, onSuccess)),
                onError => StatusCode((int)onError.HttpStatusCode, ResponseApiService.Response((int)onError.HttpStatusCode, message: onError.Description)));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "RolesEdit")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleCommand command)
        {
            command.Id = id;
            var updatedRole = await mediator.Send(command);

            return updatedRole.Match(
                onSuccess => StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, onSuccess)),
                onError => StatusCode((int)onError.HttpStatusCode, ResponseApiService.Response((int)onError.HttpStatusCode, message: onError.Description)));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "RolesEdit")]
        [HttpPut("{id:int}/actions")]
        public async Task<IActionResult> AssignActions(int id, [FromBody] AssignRoleActionsCommand command)
        {
            command.RoleId = id;
            var updatedRole = await mediator.Send(command);

            return updatedRole.Match(
                onSuccess => StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, onSuccess)),
                onError => StatusCode((int)onError.HttpStatusCode, ResponseApiService.Response((int)onError.HttpStatusCode, message: onError.Description)));
        }
    }
}
