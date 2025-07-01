using FleetManager.Application.Drivers.Query;
using FleetManager.Domain.Common.Pagination;
using MediatR;

using Microsoft.AspNetCore.Mvc;
using FleetManager.Application.Common.Features;

namespace FleetManager.Api.Controllers.Drivers
{
    [ApiController]
    [Route("api/v1/drivers")]
    public class DriversControllers(IMediator mediator) : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
        {
            var data = await mediator.Send(new GetAllDriversQuery(new PaginationParams
                    {
                        PageNumber = paginationParams.PageNumber,
                        PageSize = paginationParams.PageSize
                    }
                )
            );
            if (data is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ResponseApiService.Response(StatusCodes.Status404NotFound));
            }
            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));
        }
    }
}
