namespace Inventory.Application.Products;
public sealed record UpdateProductRequest(int Id, string Name, int CategoryId, int Stock, bool IsActive);