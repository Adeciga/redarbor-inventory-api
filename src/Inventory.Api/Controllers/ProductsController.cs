using Inventory.Application.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Inventory.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/products")]
[Produces("application/json")]
[SwaggerTag("Product management operations")]
public sealed class ProductsController : ControllerBase
{
    private const int MaxPageSize = 100;
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves products using pagination.
    /// </summary>
    /// <param name="page">1-based page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of products</returns>
    /// <response code="200">Products retrieved successfully</response>
    /// <response code="400">Invalid pagination parameters</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet]
    [SwaggerOperation(
    Summary = "Get products (paginated)",
    Description = "Returns products using page and pageSize query parameters"
)]
    [ProducesResponseType(typeof(IReadOnlyList<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest();
        }

        if (pageSize > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }


        return Ok(await _service.GetAllPagedAsync(page, pageSize, cancellationToken));
    }


    /// <summary>
    /// Retrieves a product by its identifier.
    /// </summary>
    /// <param name="id">Product identifier</param>
    /// <returns>Product information</returns>
    /// <response code="200">Product found</response>
    /// <response code="404">Product not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Get product by id",
        Description = "Returns a single product based on the provided identifier"
    )]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProductDto>> GetById(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="request">Product creation data</param>
    /// <returns>Identifier of the created product</returns>
    /// <response code="201">Product created successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a product",
        Description = "Creates a new product and returns its identifier"
    )]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Create(
        [FromBody, SwaggerRequestBody("Product data", Required = true)]
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">Product identifier</param>
    /// <param name="request">Updated product data</param>
    /// <response code="204">Product updated successfully</response>
    /// <response code="400">Identifier mismatch</response>
    /// <response code="404">Product not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Update a product",
        Description = "Updates an existing product by its identifier"
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Update(
        [FromRoute] int id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }

        var updated = await _service.UpdateAsync(request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes a product.
    /// </summary>
    /// <param name="id">Product identifier</param>
    /// <response code="204">Product deleted successfully</response>
    /// <response code="404">Product not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Delete a product",
        Description = "Deletes a product by its identifier"
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Delete(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
