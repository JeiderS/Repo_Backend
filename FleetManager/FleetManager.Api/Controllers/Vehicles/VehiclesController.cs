using FleetManager.Application.Vehicles.Query;
using FleetManager.Domain.Common.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FleetManager.Application.Common.Features;

namespace FleetManager.Api.Controllers.Vehicles
{
    [ApiController]
    [Route("api/v1/Vehicles")]
    public class VehiclesController(IMediator mediator) : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
        {
            var data = await mediator.Send(new GetAllVehiclesQuery(new PaginationParams { PageNumber = paginationParams.PageNumber, PageSize = paginationParams.PageSize }));
            if (data is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ResponseApiService.Response(StatusCodes.Status404NotFound));
            }
            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));
        }

        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var result = await mediator.Send(new DeleteVehicleCommand(id));

        //    if (!result)
        //        return NotFound(ResponseApiService.Response(StatusCodes.Status404NotFound));

        //    return NoContent();
        //}


    }
}
