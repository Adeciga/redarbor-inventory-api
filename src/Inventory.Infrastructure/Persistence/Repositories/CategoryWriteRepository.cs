using System.Data;
using Dapper;
using Inventory.Application.Categories;
using Microsoft.Data.SqlClient;

namespace Inventory.Infrastructure.Persistence.Repositories;

public sealed class CategoryWriteRepository : ICategoryWriteRepository
{
    private readonly string _connectionString;

    public CategoryWriteRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        const string sql = @"
INSERT INTO Categories (Name, IsActive)
OUTPUT INSERTED.Id
VALUES (@Name, @IsActive);";

        await using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { request.Name, request.IsActive }, cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        const string sql = @"
UPDATE Categories
SET Name = @Name,
    IsActive = @IsActive
WHERE Id = @Id;";

        await using var connection = CreateConnection();
        var affected = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { request.Id, request.Name, request.IsActive }, cancellationToken: cancellationToken));

        return affected == 1;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = @"DELETE FROM Categories WHERE Id = @Id;";

        await using var connection = CreateConnection();
        var affected = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return affected == 1;
    }

    private SqlConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);
        return connection;
    }
}
