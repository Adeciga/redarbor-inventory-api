namespace Inventory.Application.Products;

public sealed class ProductService
{
    private readonly IProductReadRepository _readRepository;
    private readonly IProductWriteRepository _writeRepository;

    public ProductService(
        IProductReadRepository readRepository,
        IProductWriteRepository writeRepository)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
    }

    public Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken) =>
        _readRepository.GetAllAsync(cancellationToken);

    public Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _readRepository.GetByIdAsync(id, cancellationToken);

    public Task<int> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken) =>
        _writeRepository.CreateAsync(request, cancellationToken);

    public Task<bool> UpdateAsync(UpdateProductRequest request, CancellationToken cancellationToken) =>
        _writeRepository.UpdateAsync(request, cancellationToken);

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken) =>
        _writeRepository.DeleteAsync(id, cancellationToken);
}

