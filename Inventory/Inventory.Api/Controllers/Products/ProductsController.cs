using Inventory.Api.Controllers.Common;
using Inventory.Application.Common.Media;
using Inventory.Application.Products.Command.CreateProduct;
using Inventory.Application.Products.Command.SetProductStatus;
using Inventory.Application.Products.Command.UpdateProduct;
using Inventory.Application.Products.Query.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers.Products
{
    [Route("api/v1/products")]
    public class ProductsController(IMediator mediator) : ApiControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "ProductosView")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await mediator.Send(new GetProductsQuery());
            return Success(products);
        }

        /// <summary>
        /// Multipart: product fields plus an optional image in the same request.
        /// The image, if present, is uploaded to Cloudinary from the backend
        /// (signed upload — see ImageUploadHelper/ICloudinaryImageUploader).
        /// </summary>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "ProductosCreate")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ProductForm form)
        {
            using var imageStream = form.Image is { Length: > 0 } ? form.Image.OpenReadStream() : null;

            var command = new CreateProductCommand
            {
                Name = form.Name,
                Description = form.Description,
                Price = form.Price,
                CategoryId = form.CategoryId,
                Image = ToImageUploadRequest(form.Image, imageStream),
            };

            var result = await mediator.Send(command);
            return FromResult(result, StatusCodes.Status201Created);
        }

        /// <summary>Same multipart shape as Create. Omitting the image keeps the product's current one.</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "ProductosEdit")]
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductForm form)
        {
            using var imageStream = form.Image is { Length: > 0 } ? form.Image.OpenReadStream() : null;

            var command = new UpdateProductCommand
            {
                Id = id,
                Name = form.Name,
                Description = form.Description,
                Price = form.Price,
                CategoryId = form.CategoryId,
                Image = ToImageUploadRequest(form.Image, imageStream),
            };

            var result = await mediator.Send(command);
            return FromResult(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "ProductosEdit")]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> SetStatus(int id, [FromBody] SetProductStatusCommand command)
        {
            command.Id = id;
            var result = await mediator.Send(command);
            return FromResult(result);
        }

        private static ImageUploadRequest? ToImageUploadRequest(IFormFile? file, Stream? content) =>
            file is null || content is null
                ? null
                : new ImageUploadRequest
                {
                    Content = content,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Length = file.Length,
                };
    }
}
