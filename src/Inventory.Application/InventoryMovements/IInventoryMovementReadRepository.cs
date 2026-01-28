namespace Inventory.Application.InventoryMovements;

public interface IInventoryMovementReadRepository
{
    Task<int> GetCurrentStockAsync(
        int productId,
        CancellationToken cancellationToken);
}
