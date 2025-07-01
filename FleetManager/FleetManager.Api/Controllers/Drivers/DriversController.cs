
using FleetManager.Application.Drivers.Query;
using FleetManager.Domain.Common.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FleetManager.Application.Common.Features;
using FleetManager.Application.Drivers.Query.GetDriversById;
using FleetManager.Application.Drivers.Errors;
using FleetManager.Application.Drivers.Commands.UpdateDrivers;
using FleetManager.Application.Drivers.Commands.CreateDrivers;
using FleetManager.Application.Drivers.Commands.DeleteDrivers;

namespace FleetManager.Api.Controllers.Drivers
{
    [ApiController]
    [Route("api/v1/drivers")]
    public class DriversController(IMediator mediator) : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
        {
            var data = await mediator.Send(new GetAllDriversQuery(new PaginationParams { PageNumber = paginationParams.PageNumber, PageSize = paginationParams.PageSize }));
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await mediator.Send(new DeleteDriversCommand(id));

            if (!result.IsSuccess)
                return NotFound(ResponseApiService.Response(StatusCodes.Status404NotFound));

            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<IResult> GetById([FromRoute] int id )
        {
            var getDriversByIdQuery = new GetDriversByIdQuery(Id: id);

            var result = await mediator.Send(getDriversByIdQuery);

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
        [HttpPost]
        public async Task<IResult> Create([FromBody] CreateDriversCommand createDriversCommand ) 
            //[FromServices] IValidator<CreateDriversRequestDto> validator)

        {
            //var validationResult = await validator.ValidateAsync(createDriversCommand.Request);

            //if (!validationResult.IsValid) return TypedResults.BadRequest(validationResult.Errors);

            var result = await mediator.Send(createDriversCommand);

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
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateDriversCommand updateDriversCommand
            )
        {
            var result = await mediator.Send(updateDriversCommand);

            if (!result.IsSuccess)
            {
                if (result.Error is DriversErrorBuilder)
                {
                    return NotFound(ResponseApiService.Response(StatusCodes.Status404NotFound));
                }

                return StatusCode(StatusCodes.Status500InternalServerError, ResponseApiService.Response(StatusCodes.Status500InternalServerError, result.Error));
            }

            return NoContent();
        }
    }
}
