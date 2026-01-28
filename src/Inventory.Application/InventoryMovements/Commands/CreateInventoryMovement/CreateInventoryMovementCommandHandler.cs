using MediatR;
using Inventory.Domain;
using Inventory.Application.InventoryMovements;

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
        // 1️⃣ Leer stock actual
        var currentStock = await _readRepository.GetCurrentStockAsync(
            request.ProductId, ct);

        // 2️⃣ Domain decide
        var item = new InventoryItem(request.ProductId, currentStock);
        item.ApplyMovement(request.Type, request.Quantity);

        // 3️⃣ Persistir movimiento
        var result = await _writeRepository.CreateAsync(
            new CreateInventoryMovementRequest(
                request.ProductId,
                request.Quantity,
                request.Type),
            ct);

        // 4️⃣ Devolver resultado del dominio
        return result ?? new InventoryMovementResult(
            request.ProductId,
            item.QuantityOnHand);
    }
}
