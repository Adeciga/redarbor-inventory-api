using Inventory.Application.InventoryMovements;
using Inventory.Domain;

public sealed record CreateInventoryMovementRequest(
    int ProductId,
    int Quantity,
    InventoryMovementType Type);

