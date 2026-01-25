namespace Inventory.Infrastructure.Persistence.Entities;

public sealed class CategoryEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
