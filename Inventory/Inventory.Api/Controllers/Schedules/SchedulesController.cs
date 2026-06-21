using Inventory.Application.Schedules.Query;
using Inventory.Domain.Common.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Inventory.Application.Common.Features;
using Inventory.Application.Schedules.Query.GetSchedulesById;
using Inventory.Application.Schedules.Errors;
using Inventory.Application.Schedules.Commands.UpdateSchedules;
using Inventory.Application.Schedules.Commands.CreateSchedules;
using Inventory.Application.Schedules.Commands.DeleteSchedules;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.Api.Controllers.Schedules
{
    [ApiController]
    [Route("api/v1/schedules")]
    public class SchedulesController(IMediator mediator) : ControllerBase
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
            var data = await mediator.Send(new GetAllSchedulesQuery(new PaginationParams { PageNumber = paginationParams.PageNumber, PageSize = paginationParams.PageSize }));
            if (data is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ResponseApiService.Response(StatusCodes.Status404NotFound));
            }
            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await mediator.Send(new DeleteSchedulesCommand(id));

            if (!result.IsSuccess)
                return NotFound(ResponseApiService.Response(StatusCodes.Status404NotFound));

            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IResult> GetById([FromRoute] int id)
        {
            var getSchedulesByIdQuery = new GetSchedulesByIdQuery(Id: id);

            var result = await mediator.Send(getSchedulesByIdQuery);

            return result.Match(
                onSuccess => TypedResults.Ok(onSuccess),
                onError => Results.Problem(onError.Description)
            );
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpPost]
        public async Task<IResult> Create([FromBody] CreateSchedulesCommand createSchedulesCommand)
        {
            var result = await mediator.Send(createSchedulesCommand);

            return result.Match(
                onSuccess => TypedResults.Ok(onSuccess),
                onError => Results.Problem(onError.Description)
            );
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateSchedulesCommand updateSchedulesCommand)
        {
            var result = await mediator.Send(updateSchedulesCommand);

            if (!result.IsSuccess)
            {
                if (result.Error is SchedulesErrorBuilder)
                {
                    return NotFound(ResponseApiService.Response(StatusCodes.Status404NotFound));
                }

                return StatusCode(StatusCodes.Status500InternalServerError, ResponseApiService.Response(StatusCodes.Status500InternalServerError, result.Error));
            }

            return NoContent();
        }
    }
}
