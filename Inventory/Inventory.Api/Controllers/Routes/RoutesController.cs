using Inventory.Application.Routes.Query;
using Inventory.Domain.Common.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Inventory.Application.Common.Features;
using Inventory.Application.Routes.Commands.CreateRoutes;
using Inventory.Application.Routes.Commands.UpdateRoutes;
using Inventory.Application.Routes.Errors;
using Inventory.Application.Routes.Query.GetRoutesById;
using Inventory.Application.Routes.Commands.DeleteRoutes;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.Api.Controllers.Routes
{

    [ApiController]
    [Route("api/v1/routes")]
    public class RoutesController(IMediator mediator) :  ControllerBase
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
            var data = await mediator.Send(new GetAllRoutesQuery(new PaginationParams { PageNumber = paginationParams.PageNumber, PageSize = paginationParams.PageSize }));
            if (data is null)
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
            var result = await mediator.Send(new DeleteRoutesCommand(id));

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
            var getRoutesByIdQuery = new GetRoutesByIdQuery(Id: id);

            var result = await mediator.Send(getRoutesByIdQuery);

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
        public async Task<IResult> Create([FromBody] CreateRoutesCommand createRoutesCommand)
        //[FromServices] IValidator<CreateDriversRequestDto> validator)
        {

            //var validationResult = await validator.ValidateAsync(createDriversCommand.Request);

            //if (!validationResult.IsValid) return TypedResults.BadRequest(validationResult.Errors);

            var result = await mediator.Send(createRoutesCommand);

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
        public async Task<IActionResult> Update([FromBody] UpdateRoutesCommand updateRoutesCommand)
        {
            var result = await mediator.Send(updateRoutesCommand);

            if (!result.IsSuccess)
            {
                if (result.Error.Code == RoutesErrorBuilder.ROUTE_NOT_FOUND_ERROR)
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

