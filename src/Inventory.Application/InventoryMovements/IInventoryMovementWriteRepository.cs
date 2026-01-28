using Inventory.Application.InventoryMovements;

public interface IInventoryMovementWriteRepository
{
    Task<InventoryMovementResult?> CreateAsync(
        CreateInventoryMovementRequest request,
        CancellationToken cancellationToken);
}
