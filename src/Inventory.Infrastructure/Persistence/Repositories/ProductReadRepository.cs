using Inventory.Application.Products;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public sealed class ProductReadRepository : IProductReadRepository
{
    private readonly InventoryDbContext _dbContext;

    public ProductReadRepository(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await _dbContext.Products
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new ProductDto(x.Id, x.Name, x.CategoryId, x.Stock, x.IsActive))
            .ToListAsync(cancellationToken);

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        await _dbContext.Products
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ProductDto(x.Id, x.Name, x.CategoryId, x.Stock, x.IsActive))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<ProductDto>> GetAllPagedAsync(
    int page,
    int pageSize,
    CancellationToken cancellationToken)
    {
        var skip = (page - 1) * pageSize;

        return await _dbContext.Products
            .OrderBy(p => p.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.CategoryId,
                p.Stock,
                p.IsActive))
            .ToListAsync(cancellationToken);
    }

}
