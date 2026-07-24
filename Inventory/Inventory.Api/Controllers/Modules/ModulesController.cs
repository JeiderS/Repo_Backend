using System.Security.Claims;
using Inventory.Application.Common.Features;
using Inventory.Application.Modules.Command.CreateModule;
using Inventory.Application.Modules.Command.DeactivateModule;
using Inventory.Application.Modules.Command.UpdateModule;
using Inventory.Application.Modules.Query.GetModuleMenu;
using Inventory.Application.Modules.Query.GetModules;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers.Modules
{
    [ApiController]
    [Route("api/v1/modules")]
    public class ModulesController(IMediator mediator) : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpGet("menu")]
        public async Task<IActionResult> GetMenu()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var menu = await mediator.Send(new GetModuleMenuQuery(userId));

            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, menu));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var modules = await mediator.Send(new GetModulesQuery());

            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, modules));
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateModuleCommand command)
        {
            var createdModule = await mediator.Send(command);

            return createdModule.Match(
                onSuccess => StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, onSuccess)),
                onError => StatusCode((int)onError.HttpStatusCode, ResponseApiService.Response((int)onError.HttpStatusCode, message: onError.Description)));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateModuleCommand command)
        {
            command.Id = id;
            var updatedModule = await mediator.Send(command);

            return updatedModule.Match(
                onSuccess => StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, onSuccess)),
                onError => StatusCode((int)onError.HttpStatusCode, ResponseApiService.Response((int)onError.HttpStatusCode, message: onError.Description)));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var deactivatedModule = await mediator.Send(new DeactivateModuleCommand(id));

            return deactivatedModule.Match(
                onSuccess => StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, onSuccess)),
                onError => StatusCode((int)onError.HttpStatusCode, ResponseApiService.Response((int)onError.HttpStatusCode, message: onError.Description)));
        }
    }
}
