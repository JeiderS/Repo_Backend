using System.Security.Claims;
using Inventory.Api.Auth;
using Inventory.Api.Controllers.Common;
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
    [Route("api/v1/modules")]
    public class ModulesController(IMediator mediator) : ApiControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpGet("menu")]
        public async Task<IActionResult> GetMenu()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var menu = await mediator.Send(new GetModuleMenuQuery(userId));
            return Success(menu);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Policy = AuthorizationPolicies.SystemAdminOnly)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var modules = await mediator.Send(new GetModulesQuery());
            return Success(modules);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Policy = AuthorizationPolicies.SystemAdminOnly)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateModuleCommand command)
        {
            var result = await mediator.Send(command);
            return FromResult(result, StatusCodes.Status201Created);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Policy = AuthorizationPolicies.SystemAdminOnly)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateModuleCommand command)
        {
            command.Id = id;
            var result = await mediator.Send(command);
            return FromResult(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Policy = AuthorizationPolicies.SystemAdminOnly)]
        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await mediator.Send(new DeactivateModuleCommand(id));
            return FromResult(result);
        }
    }
}
