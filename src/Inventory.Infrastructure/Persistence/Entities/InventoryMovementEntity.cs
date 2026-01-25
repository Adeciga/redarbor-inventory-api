namespace Inventory.Infrastructure.Persistence.Entities;

public sealed class InventoryMovementEntity
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string Type { get; set; } = string.Empty;
}

