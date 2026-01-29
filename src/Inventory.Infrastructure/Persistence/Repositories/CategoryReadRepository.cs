using Inventory.Application.Categories;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public sealed class CategoryReadRepository : ICategoryReadRepository
{
    private readonly InventoryDbContext _dbContext;

    public CategoryReadRepository(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await _dbContext.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CategoryDto(x.Id, x.Name, x.IsActive))
            .ToListAsync(cancellationToken);

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        await _dbContext.Categories
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new CategoryDto(x.Id, x.Name, x.IsActive))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<CategoryDto>> GetAllPagedAsync(
       int page,
       int pageSize,
       CancellationToken cancellationToken)
    {
        var skip = (page - 1) * pageSize;

        return await _dbContext.Categories
            .OrderBy(x => x.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(x => new CategoryDto(
                x.Id,
                x.Name,
                x.IsActive))
            .ToListAsync(cancellationToken);
    }

}
