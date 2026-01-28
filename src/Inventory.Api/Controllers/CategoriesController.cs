using Inventory.Application.Categories;
using Inventory.Application.Categories.Commands.CreateCategory;
using Inventory.Application.Categories.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly CategoryService _service;

    public CategoriesController(IMediator mediator, CategoryService service)
    {
        _mediator = mediator;
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetCategoriesQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken cancellationToken)
    {
        // TODO Fase 1.1: Migrar a GetCategoryByIdQuery
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new CreateCategoryCommand(request.Name, request.IsActive),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        // TODO Fase 1.1: Migrar a UpdateCategoryCommand
        if (id != request.Id)
        {
            return BadRequest();
        }

        var updated = await _service.UpdateAsync(request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        // TODO Fase 1.1: Migrar a DeleteCategoryCommand
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
