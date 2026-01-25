namespace Inventory.Application.Products;

public interface IProductWriteRepository
{
    Task<int> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(UpdateProductRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
