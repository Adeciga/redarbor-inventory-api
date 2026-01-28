using Inventory.Domain;
using Xunit;

public class InventoryItemTests
{
    [Fact]
    public void Increases_stock_when_movement_is_in()
    {
        var item = new InventoryItem(productId: 1, initialQuantity: 10);

        item.ApplyMovement(InventoryMovementType.In, 5);

        Assert.Equal(15, item.QuantityOnHand);
    }

    [Fact]
    public void Decreases_stock_when_movement_is_out()
    {
        var item = new InventoryItem(1, 10);

        item.ApplyMovement(InventoryMovementType.Out, 3);

        Assert.Equal(7, item.QuantityOnHand);
    }

    [Fact]
    public void Throws_when_quantity_is_zero_or_negative()
    {
        var item = new InventoryItem(1, 10);

        Assert.Throws<DomainException>(() =>
            item.ApplyMovement(InventoryMovementType.In, 0));
    }

    [Fact]
    public void Throws_when_insufficient_stock()
    {
        var item = new InventoryItem(1, 5);

        Assert.Throws<DomainException>(() =>
            item.ApplyMovement(InventoryMovementType.Out, 10));
    }
}
