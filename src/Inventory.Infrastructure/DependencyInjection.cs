using Inventory.Application.Categories;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Inventory.Application.Products;
using Inventory.Application.InventoryMovements;



namespace Inventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<InventoryDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        var connectionString = configuration.GetConnectionString("Default") ?? string.Empty;

        services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();
        services.AddScoped<ICategoryWriteRepository>(_ => new CategoryWriteRepository(connectionString));

        services.AddScoped<IProductReadRepository, ProductReadRepository>();
        services.AddScoped<IProductWriteRepository>(_ => new ProductWriteRepository(connectionString));

        services.AddScoped<IInventoryMovementWriteRepository>(_ => new InventoryMovementWriteRepository(connectionString));



        return services;
    }
}
