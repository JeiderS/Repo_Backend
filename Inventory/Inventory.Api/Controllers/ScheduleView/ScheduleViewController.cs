using FleetManager.Application.ScheduleView.Query;
using FleetManager.Domain.Common.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FleetManager.Application.Common.Features;

namespace FleetManager.Api.Controllers.ScheduleView
{
    [ApiController]
    [Route("api/v1/schedule-view")]
    public class ScheduleViewController(IMediator mediator) : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
        {
            var data = await mediator.Send(new GetAllScheduleViewQuery(
                new PaginationParams
                {
                    PageNumber = paginationParams.PageNumber,
                    PageSize = paginationParams.PageSize
                }));

            if (data is null || !data.Any())
                return NotFound(ResponseApiService.Response(StatusCodes.Status404NotFound));

            return Ok(ResponseApiService.Response(StatusCodes.Status200OK, data));
        }
    }
}
