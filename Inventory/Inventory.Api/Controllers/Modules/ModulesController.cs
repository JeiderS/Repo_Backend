using System.Security.Claims;
using Inventory.Application.Common.Features;
using Inventory.Application.Modules.Query.GetModuleMenu;
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
    }
}
