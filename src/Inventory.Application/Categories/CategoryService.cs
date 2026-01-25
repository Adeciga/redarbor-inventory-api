namespace Inventory.Application.Categories;

public sealed class CategoryService
{
    private readonly ICategoryReadRepository _readRepository;
    private readonly ICategoryWriteRepository _writeRepository;

    public CategoryService(
        ICategoryReadRepository readRepository,
        ICategoryWriteRepository writeRepository)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
    }

    public Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken) =>
        _readRepository.GetAllAsync(cancellationToken);

    public Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _readRepository.GetByIdAsync(id, cancellationToken);

    public Task<int> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken) =>
        _writeRepository.CreateAsync(request, cancellationToken);

    public Task<bool> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken) =>
        _writeRepository.UpdateAsync(request, cancellationToken);

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken) =>
        _writeRepository.DeleteAsync(id, cancellationToken);
}

