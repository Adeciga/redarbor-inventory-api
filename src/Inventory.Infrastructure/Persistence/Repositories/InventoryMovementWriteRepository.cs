using Dapper;
using Inventory.Application.InventoryMovements;
using Inventory.Domain;
using Microsoft.Data.SqlClient;

namespace Inventory.Infrastructure.Persistence.Repositories;

public sealed class InventoryMovementWriteRepository : IInventoryMovementWriteRepository
{
    private readonly string _connectionString;

    public InventoryMovementWriteRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<InventoryMovementResult?> CreateAsync(CreateInventoryMovementRequest request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            return null;
        }

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var transaction = connection.BeginTransaction();

        try
        {
            var currentStock = await GetCurrentStockAsync(connection, transaction, request.ProductId, cancellationToken);

            if (currentStock is null)
            {
                transaction.Rollback();
                return null;
            }

            var newStock = CalculateNewStock(currentStock.Value, request);

            if (newStock < 0)
            {
                transaction.Rollback();
                return null;
            }

            var movementType = request.Type == InventoryMovementType.In ? "IN" : "OUT";

            await InsertMovementAsync(connection, transaction, request, movementType, cancellationToken);
            await UpdateStockAsync(connection, transaction, request.ProductId, newStock, cancellationToken);

            transaction.Commit();
            return new InventoryMovementResult(request.ProductId, newStock);
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static async Task<int?> GetCurrentStockAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        int productId,
        CancellationToken cancellationToken)
    {
        const string sql = @"SELECT Stock FROM Products WHERE Id = @Id;";

        return await connection.ExecuteScalarAsync<int?>(
            new CommandDefinition(sql, new { Id = productId }, transaction, cancellationToken: cancellationToken));
    }

    private static int CalculateNewStock(int currentStock, CreateInventoryMovementRequest request) =>
        request.Type == InventoryMovementType.In
            ? currentStock + request.Quantity
            : currentStock - request.Quantity;

    private static async Task InsertMovementAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        CreateInventoryMovementRequest request,
        string type,
        CancellationToken cancellationToken)
    {
        const string sql = @"
INSERT INTO InventoryMovements (ProductId, Quantity, Type, CreatedAtUtc)
VALUES (@ProductId, @Quantity, @Type, @CreatedAtUtc);";

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    request.ProductId,
                    request.Quantity,
                    Type = type,
                    CreatedAtUtc = DateTime.UtcNow
                },
                transaction,
                cancellationToken: cancellationToken));
    }

    private static async Task UpdateStockAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        int productId,
        int newStock,
        CancellationToken cancellationToken)
    {
        const string sql = @"UPDATE Products SET Stock = @Stock WHERE Id = @Id;";

        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Stock = newStock, Id = productId }, transaction, cancellationToken: cancellationToken));
    }
}
