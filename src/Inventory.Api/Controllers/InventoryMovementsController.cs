using Inventory.Application.InventoryMovements;
using Inventory.Application.InventoryMovements.Commands.CreateInventoryMovement;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/inventory-movements")]
public sealed class InventoryMovementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryMovementsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateInventoryMovementRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CreateInventoryMovementCommand(
                request.ProductId,
                request.Quantity,
                request.Type),
            ct);

        return Ok(result);
    }
}
