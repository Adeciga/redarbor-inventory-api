namespace Inventory.Application.InventoryMovements;

public sealed record CreateInventoryMovementRequest(int ProductId, int Quantity, InventoryMovementType Type);
