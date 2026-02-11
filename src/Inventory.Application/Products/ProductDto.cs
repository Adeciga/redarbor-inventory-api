namespace Inventory.Application.Products;
public sealed record ProductDto(int Id, string Name, int CategoryId, int Stock, bool IsActive);