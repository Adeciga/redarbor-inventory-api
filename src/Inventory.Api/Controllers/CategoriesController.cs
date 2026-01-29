using Inventory.Application.Categories;
using Inventory.Application.Categories.Commands.CreateCategory;
using Inventory.Application.Categories.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Inventory.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/categories")]
[Produces("application/json")]
[SwaggerTag("Category management operations")]
public sealed class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly CategoryService _service;

    public CategoriesController(IMediator mediator, CategoryService service)
    {
        _mediator = mediator;
        _service = service;
    }

    /// <summary>
    /// Retrieves categories using pagination.
    /// </summary>
    /// <param name="page">1-based page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of categories</returns>
    /// <response code="200">Categories retrieved successfully</response>
    /// <response code="400">Invalid pagination parameters</response>
    /// <response code="401">Unauthorized</response>
    private const int MaxPageSize = 100;

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get categories (paginated)",
        Description = "Returns categories using page and pageSize query parameters"
    )]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(
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
    /// Retrieves a category by its identifier.
    /// </summary>
    /// <param name="id">Category identifier</param>
    /// <returns>Category information</returns>
    /// <response code="200">Category found</response>
    /// <response code="404">Category not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Get category by id",
        Description = "Returns a single category based on the provided identifier"
    )]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CategoryDto>> GetById(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="request">Category creation data</param>
    /// <returns>Identifier of the created category</returns>
    /// <response code="201">Category created successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a category",
        Description = "Creates a new category and returns its identifier"
    )]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Create(
        [FromBody, SwaggerRequestBody("Category data", Required = true)]
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new CreateCategoryCommand(request.Name, request.IsActive),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    /// <param name="id">Category identifier</param>
    /// <param name="request">Updated category data</param>
    /// <response code="204">Category updated successfully</response>
    /// <response code="400">Identifier mismatch</response>
    /// <response code="404">Category not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Update a category",
        Description = "Updates an existing category by its identifier"
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Update(
        [FromRoute] int id,
        [FromBody] UpdateCategoryRequest request,
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
    /// Deletes a category.
    /// </summary>
    /// <param name="id">Category identifier</param>
    /// <response code="204">Category deleted successfully</response>
    /// <response code="404">Category not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Delete a category",
        Description = "Deletes a category by its identifier"
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
