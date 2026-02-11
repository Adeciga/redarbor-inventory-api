namespace Inventory.Application.Products;
public interface IProductReadRepository
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductDto>> GetAllPagedAsync(
    int page,
    int pageSize,
    CancellationToken cancellationToken);
}