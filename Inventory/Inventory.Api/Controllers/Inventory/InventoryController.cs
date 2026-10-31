using Inventory.Api.Controllers.Common;
using Inventory.Application.Inventories.Command.RegisterInventoryIn;
using Inventory.Application.Inventories.Command.RegisterInventoryOut;
using Inventory.Application.Inventories.Query.GetInventoryOverview;
using Inventory.Application.InventoryMovements.Query.GetInventoryMovements;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers.Inventory
{
    [Route("api/v1/inventory")]
    public class InventoryController(IMediator mediator) : ApiControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "InventarioView")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var inventory = await mediator.Send(new GetInventoryOverviewQuery());
            return Success(inventory);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "InventarioView")]
        [HttpGet("movements")]
        public async Task<IActionResult> GetMovements()
        {
            var movements = await mediator.Send(new GetInventoryMovementsQuery());
            return Success(movements);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "InventarioRegisterIn")]
        [HttpPost("{id:int}/movements/in")]
        public async Task<IActionResult> RegisterIn(int id, [FromBody] RegisterInventoryInCommand command)
        {
            command.InventoryId = id;
            var result = await mediator.Send(command);
            return FromResult(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "InventarioRegisterOut")]
        [HttpPost("{id:int}/movements/out")]
        public async Task<IActionResult> RegisterOut(int id, [FromBody] RegisterInventoryOutCommand command)
        {
            command.InventoryId = id;
            var result = await mediator.Send(command);
            return FromResult(result);
        }
    }
}
