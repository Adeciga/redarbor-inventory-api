using Inventory.Application.InventoryMovements.Commands.CreateInventoryMovement;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
namespace Inventory.Api.Controllers;
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/inventory-movements")]
[Produces("application/json")]
[SwaggerTag("Inventory movement operations")]
public sealed class InventoryMovementsController : ControllerBase
{
    private readonly IMediator _mediator;
    public InventoryMovementsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    /// <summary>
    /// Registers an inventory movement (inbound or outbound) for a product.
    /// </summary>
    /// <remarks>
    /// This endpoint records a stock movement for a product.
    /// The movement type determines whether the quantity is added or removed from inventory.
    ///
    /// Sample request:
    ///
    ///     POST /api/inventory-movements
    ///     {
    ///         "productId": 10,
    ///         "quantity": 5,
    ///         "type": "IN"
    ///     }
    ///
    /// </remarks>
    /// <param name="request">Inventory movement data</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    /// <response code="204">Inventory movement registered successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create inventory movement",
        Description = "Registers a new inventory movement for a product"
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        [FromBody, SwaggerRequestBody("Inventory movement data", Required = true)]
        CreateInventoryMovementRequest request,
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
