namespace Inventory.Application.InventoryMovements;
public sealed class InventoryMovementService
{
    private readonly IInventoryMovementWriteRepository _writeRepository;
    public InventoryMovementService(IInventoryMovementWriteRepository writeRepository)
    {
        _writeRepository = writeRepository;
    }
    public Task<InventoryMovementResult?> CreateAsync(CreateInventoryMovementRequest request, CancellationToken cancellationToken) =>
        _writeRepository.CreateAsync(request, cancellationToken);
}