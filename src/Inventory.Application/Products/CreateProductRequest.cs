namespace Inventory.Application.Products;

public sealed record CreateProductRequest(string Name, int CategoryId, int Stock, bool IsActive);
