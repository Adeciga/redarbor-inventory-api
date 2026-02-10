using MediatR;
using Inventory.Domain;
namespace Inventory.Application.InventoryMovements.Commands.CreateInventoryMovement;
public sealed class CreateInventoryMovementCommandHandler
    : IRequestHandler<CreateInventoryMovementCommand, InventoryMovementResult>
{
    private readonly IInventoryMovementWriteRepository _writeRepository;
    private readonly IInventoryMovementReadRepository _readRepository;
    public CreateInventoryMovementCommandHandler(
        IInventoryMovementWriteRepository writeRepository,
        IInventoryMovementReadRepository readRepository)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
    }
    public async Task<InventoryMovementResult> Handle(
        CreateInventoryMovementCommand request,
        CancellationToken ct)
    {
        var currentStock = await _readRepository.GetCurrentStockAsync(
            request.ProductId, ct);
        var item = new InventoryItem(request.ProductId, currentStock);
        item.ApplyMovement(request.Type, request.Quantity);
        var result = await _writeRepository.CreateAsync(
            new CreateInventoryMovementRequest(
                request.ProductId,
                request.Quantity,
                request.Type),
            ct);
        return result ?? new InventoryMovementResult(
            request.ProductId,
            item.QuantityOnHand);
    }
}
