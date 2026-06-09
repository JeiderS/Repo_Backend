using FleetManager.Application.Vehicles.Query;
using FleetManager.Domain.Common.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FleetManager.Application.Common.Features;
using FleetManager.Application.Vehicles.Commands.CreateVehicles;
using FleetManager.Application.Vehicles.Commands.DeleteVehicles;
using FleetManager.Application.Vehicles.Commands.UpdateVehicles;
using FleetManager.Application.Vehicles.Errors;
using FleetManager.Application.Vehicles.Query.GetVehiclesById;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await mediator.Send(new DeleteVehiclesCommand(id));

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
            var getVehiclesByIdQuery = new GetVehiclesByIdQuery(Id: id);

            var result = await mediator.Send(getVehiclesByIdQuery);

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
        public async Task<IResult> Create([FromBody] CreateVehiclesCommand createVehiclesCommand)
        //[FromServices] IValidator<CreateDriversRequestDto> validator)
        {


            //var validationResult = await validator.ValidateAsync(createDriversCommand.Request);

            //if (!validationResult.IsValid) return TypedResults.BadRequest(validationResult.Errors);
            var result = await mediator.Send(createVehiclesCommand);

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
        public async Task<IActionResult> Update([FromBody] UpdateVehiclesCommand updateVehiclesCommand)
        {
            var result = await mediator.Send(updateVehiclesCommand);

            if (!result.IsSuccess)
            {
                if (result.Error.Code == VehiclesErrorBuilder.VEHICLE_NOT_FOUND_ERROR)
                {
                    return NotFound(ResponseApiService.Response(StatusCodes.Status404NotFound));
                }

                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseApiService.Response(StatusCodes.Status500InternalServerError, result.Error));
            }

            return NoContent();
        }



    }
}
