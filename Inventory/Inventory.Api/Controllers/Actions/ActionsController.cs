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
    }
}
