using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using TaskManagement.Application.Entities;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Infrastructure.Repositories;

public class DapperTaskRepository : ITaskRepository
{
    private readonly string _connectionString;

    public DapperTaskRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                           ?? throw new InvalidOperationException("Connection string is missing!");
    }

    private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

    public async Task<IEnumerable<TaskEntity>> GetAllAsync(int? status = null)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM public.\"Tasks\"";

        if (status.HasValue)
        {
            sql += " WHERE \"Status\" = @Status";
            return await connection.QueryAsync<TaskEntity>(sql, new { Status = status.Value });
        }

        return await connection.QueryAsync<TaskEntity>(sql);
    }


    public async Task<TaskEntity?> GetByIdAsync(Guid id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM public.\"Tasks\" WHERE \"Id\" = @Id"; // ✅ Кавычки для столбца
        return await connection.QueryFirstOrDefaultAsync<TaskEntity>(sql, new { Id = id });
    }

    public async Task<Guid> CreateAsync(TaskEntity task)
    {
        using var connection = CreateConnection();
        var sql = @"
        INSERT INTO public.""Tasks"" (""Id"", ""Title"", ""Description"", ""Status"", ""CreatedAt"", ""UpdatedAt"")
        VALUES (@Id, @Title, @Description, @Status, @CreatedAt, @UpdatedAt)";

        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        await connection.ExecuteAsync(sql, task);
        return task.Id;
    }


    public async Task<bool> UpdateAsync(TaskEntity task)
    {
        using var connection = CreateConnection();
        var sql = @"
            UPDATE public.""Tasks""
            SET ""Title"" = @Title, 
                ""Description"" = @Description, 
                ""Status"" = @Status, 
                ""UpdatedAt"" = CASE 
                    WHEN ""Status"" != @Status THEN NOW()
                    ELSE ""UpdatedAt""
                END
            WHERE ""Id"" = @Id";

        var rowsAffected = await connection.ExecuteAsync(sql, task);
        return rowsAffected > 0;
    }


    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM public.\"Tasks\" WHERE \"Id\" = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}
