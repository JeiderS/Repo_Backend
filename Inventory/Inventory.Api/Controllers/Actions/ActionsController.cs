using Inventory.Application.Actions.Command.CreateAction;
using Inventory.Application.Actions.Query.GetActions;
using Inventory.Application.Common.Features;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers.Actions
{
    [ApiController]
    [Route("api/v1/actions")]
    [Authorize(Roles = "RolesView")]
    public class ActionsController(IMediator mediator) : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var actions = await mediator.Send(new GetActionsQuery());

            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, actions));
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "RolesEdit")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateActionCommand command)
        {
            var createdAction = await mediator.Send(command);

            return createdAction.Match(
                onSuccess => StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, onSuccess)),
                onError => StatusCode((int)onError.HttpStatusCode, ResponseApiService.Response((int)onError.HttpStatusCode, message: onError.Description)));
        }
    }
}
