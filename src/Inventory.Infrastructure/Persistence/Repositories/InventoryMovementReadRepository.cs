using Dapper;
using Inventory.Application.InventoryMovements;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public sealed class InventoryMovementReadRepository : IInventoryMovementReadRepository
{
    private readonly InventoryDbContext _db;

    public InventoryMovementReadRepository(InventoryDbContext db) => _db = db;

    public async Task<int> GetCurrentStockAsync(int productId, CancellationToken cancellationToken)
    {
        // Opción simple: el stock viene del producto
        // Ajusta el nombre del campo/entidad según tu modelo real.
        var product = await _db.Products
            .AsNoTracking()
            .Where(p => p.Id == productId)
            .Select(p => new { p.Stock })
            .FirstOrDefaultAsync(cancellationToken);

        return product?.Stock ?? 0;
    }
}
