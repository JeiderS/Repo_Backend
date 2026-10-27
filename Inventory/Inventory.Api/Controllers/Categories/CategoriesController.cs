using Inventory.Api.Controllers.Common;
using Inventory.Application.Categories.Command.CreateCategory;
using Inventory.Application.Categories.Command.SetCategoryStatus;
using Inventory.Application.Categories.Command.UpdateCategory;
using Inventory.Application.Categories.Query.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers.Categories
{
    [Route("api/v1/categories")]
    public class CategoriesController(IMediator mediator) : ApiControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "CategoríasView, ProductosView")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await mediator.Send(new GetCategoriesQuery());
            return Success(categories);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "CategoríasCreate")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
        {
            var result = await mediator.Send(command);
            return FromResult(result, StatusCodes.Status201Created);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "CategoríasEdit")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryCommand command)
        {
            command.Id = id;
            var result = await mediator.Send(command);
            return FromResult(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "CategoríasEdit")]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> SetStatus(int id, [FromBody] SetCategoryStatusCommand command)
        {
            command.Id = id;
            var result = await mediator.Send(command);
            return FromResult(result);
        }
    }
}
