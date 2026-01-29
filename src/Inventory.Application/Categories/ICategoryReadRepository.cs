namespace Inventory.Application.Categories;

public interface ICategoryReadRepository
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<CategoryDto>> GetAllPagedAsync(
    int page,
    int pageSize,
    CancellationToken cancellationToken);

}
