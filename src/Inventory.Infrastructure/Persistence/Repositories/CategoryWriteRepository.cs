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
        const string sql = @"INSERT INTO Categories (Name, IsActive) OUTPUT INSERTED.Id VALUES (@Name, @IsActive);";
        await using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { request.Name, request.IsActive }, cancellationToken: cancellationToken));
    }
    public async Task<bool> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        const string sql = @"UPDATE Categories SET Name = @Name, IsActive = @IsActive WHERE Id = @Id;";
        await using var connection = CreateConnection();
        var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new { request.Id, request.Name, request.IsActive }, cancellationToken: cancellationToken));
        return affected == 1;
    }
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        const string softDeleteSql = @"UPDATE Categories SET IsDeleted = 1, DeletedAtUtc = SYSUTCDATETIME(), DeletedBy = @DeletedBy WHERE Id = @Id AND IsDeleted = 0;";
        const string existsSql = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM Categories WHERE Id = @Id) THEN 1 ELSE 0 END;";
        await using var connection = CreateConnection();
        var affected = await connection.ExecuteAsync(new CommandDefinition(softDeleteSql, new { Id = id, DeletedBy = "system" }, cancellationToken: cancellationToken));
        if (affected == 1)
        {
            return true;
        }
        var exists = await connection.ExecuteScalarAsync<int>(new CommandDefinition(existsSql, new { Id = id }, cancellationToken: cancellationToken));
        return exists == 1;
    }
    private SqlConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);
        return connection;
    }
}
