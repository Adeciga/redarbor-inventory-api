namespace Inventory.Application.Categories;
public interface ICategoryWriteRepository
{
    Task<int> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
