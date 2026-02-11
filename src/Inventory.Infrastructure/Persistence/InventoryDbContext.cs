using Inventory.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public sealed class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<InventoryMovementEntity> InventoryMovements => Set<InventoryMovementEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoryEntity>()
            .Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<CategoryEntity>()
            .Property(x => x.DeletedBy)
            .HasMaxLength(100);

        modelBuilder.Entity<CategoryEntity>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<ProductEntity>()
            .Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<ProductEntity>()
            .Property(x => x.DeletedBy)
            .HasMaxLength(100);

        modelBuilder.Entity<ProductEntity>()
            .HasQueryFilter(x => !x.IsDeleted);
    }
}
