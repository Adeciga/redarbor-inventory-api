namespace Inventory.Domain;

public sealed class InventoryItem
{
    public int ProductId { get; }
    public int QuantityOnHand { get; private set; }

    public InventoryItem(int productId, int initialQuantity)
    {
        if (initialQuantity < 0)
            throw new DomainException("Initial quantity cannot be negative.");

        ProductId = productId;
        QuantityOnHand = initialQuantity;
    }

    public void ApplyMovement(InventoryMovementType type, int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (type == InventoryMovementType.Out && QuantityOnHand < quantity)
            throw new DomainException("Insufficient stock.");

        QuantityOnHand = type switch
        {
            InventoryMovementType.In => QuantityOnHand + quantity,
            InventoryMovementType.Out => QuantityOnHand - quantity,
            _ => throw new DomainException("Invalid movement type.")
        };
    }
}
