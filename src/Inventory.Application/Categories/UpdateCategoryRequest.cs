namespace Inventory.Application.Categories;
public sealed record UpdateCategoryRequest(int Id, string Name, bool IsActive);