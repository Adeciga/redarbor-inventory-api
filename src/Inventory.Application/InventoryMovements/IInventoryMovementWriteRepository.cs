public interface IInventoryMovementWriteRepository
{
    Task<InventoryMovementResult?> CreateAsync(CreateInventoryMovementRequest request,CancellationToken cancellationToken);
}