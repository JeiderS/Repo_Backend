using Inventory.Api.Controllers.Common;
using Inventory.Application.Actions.Command.CreateAction;
using Inventory.Application.Actions.Command.DeleteAction;
using Inventory.Application.Actions.Command.UpdateAction;
using Inventory.Application.Actions.Query.GetActions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers.Actions
{
    [Route("api/v1/actions")]
    public class ActionsController(IMediator mediator) : ApiControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "AccionesView, RolesView, MódulosView")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var actions = await mediator.Send(new GetActionsQuery());
            return Success(actions);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "AccionesCreate")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateActionCommand command)
        {
            var result = await mediator.Send(command);
            return FromResult(result, StatusCodes.Status201Created);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "AccionesEdit")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateActionCommand command)
        {
            command.Id = id;
            var result = await mediator.Send(command);
            return FromResult(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "AccionesDelete")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await mediator.Send(new DeleteActionCommand(id));
            return FromResult(result);
        }
    }
}
