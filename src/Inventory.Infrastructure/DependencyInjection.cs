using Inventory.Application.Behaviors;
using Inventory.Application.Categories;
using Inventory.Application.InventoryMovements;
using Inventory.Application.Products;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Persistence.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



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

        services.AddScoped<IInventoryMovementReadRepository, InventoryMovementReadRepository>();
        services.AddScoped<IInventoryMovementWriteRepository>(_ => new InventoryMovementWriteRepository(connectionString));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        return services;
    }
}
