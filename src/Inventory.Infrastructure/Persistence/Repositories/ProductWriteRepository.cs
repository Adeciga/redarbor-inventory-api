using Dapper;
using Inventory.Application.Products;
using Microsoft.Data.SqlClient;

namespace Inventory.Infrastructure.Persistence.Repositories;

public sealed class ProductWriteRepository : IProductWriteRepository
{
    private readonly string _connectionString;

    public ProductWriteRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    public async Task<int> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO Products (Name, CategoryId, Stock, IsActive) OUTPUT INSERTED.Id VALUES (@Name, @CategoryId, @Stock, @IsActive);";
        await using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { request.Name, request.CategoryId, request.Stock, request.IsActive }, cancellationToken: cancellationToken));
    }
    public async Task<bool> UpdateAsync(UpdateProductRequest request, CancellationToken cancellationToken)
    {
        const string sql = @"UPDATE Products SET Name = @Name, CategoryId = @CategoryId, Stock = @Stock, IsActive = @IsActive WHERE Id = @Id;";
        await using var connection = CreateConnection();
        var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new { request.Id, request.Name, request.CategoryId, request.Stock, request.IsActive }, cancellationToken: cancellationToken));
        return affected == 1;
    }
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        const string softDeleteSql = @"UPDATE Products SET IsDeleted = 1, DeletedAtUtc = SYSUTCDATETIME(), DeletedBy = @DeletedBy WHERE Id = @Id AND IsDeleted = 0;";
        const string existsSql = @"SELECT CASE WHEN EXISTS ( SELECT 1 FROM Products WHERE Id = @Id ) THEN 1 ELSE 0 END;";
        await using var connection = CreateConnection();
        var affected = await connection.ExecuteAsync(new CommandDefinition(softDeleteSql, new { Id = id, DeletedBy = "system" }, cancellationToken: cancellationToken));
        if (affected == 1)
        {
            return true;
        }
        var exists = await connection.ExecuteScalarAsync<int>(new CommandDefinition(existsSql, new { Id = id }, cancellationToken: cancellationToken));
        return exists == 1;
    }
    private SqlConnection CreateConnection() => new(_connectionString);
}
