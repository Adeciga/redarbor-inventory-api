using Inventory.Application.InventoryMovements;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/inventory-movements")]
public sealed class InventoryMovementsController : ControllerBase
{
    private readonly InventoryMovementService _service;

    public InventoryMovementsController(InventoryMovementService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<InventoryMovementResult>> Create(
        CreateInventoryMovementRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(request, cancellationToken);

        return result is null ? BadRequest() : Ok(result);
    }
}
