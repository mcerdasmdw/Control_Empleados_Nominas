using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Planilla_RRHH.Data;

namespace Planilla_RRHH.Data.Repositories;

public sealed class PasswordResetRepository : IPasswordResetRepository
{
    private readonly Database _database;
    private readonly ILogger<PasswordResetRepository>? _logger;
    
    public PasswordResetRepository(Database database, ILogger<PasswordResetRepository>? logger = null)
    {
        _database = database;
        _logger = logger;
    }
    
    public async Task<PasswordResetToken?> GetByTokenHashAsync(byte[] tokenHash)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT Id, UserId, TokenHash, ExpiresAt, UsedAt, CreatedAt FROM PasswordResetTokens WHERE TokenHash = @hash AND UsedAt IS NULL AND ExpiresAt > @now;";
        command.Parameters.AddWithValue("@hash", tokenHash);
        command.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("O"));
        
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadToken(reader);
        }
        return null;
    }
    
    public async Task<PasswordResetToken?> GetByUserIdAsync(int userId)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT Id, UserId, TokenHash, ExpiresAt, UsedAt, CreatedAt FROM PasswordResetTokens WHERE UserId = @userId AND UsedAt IS NULL AND ExpiresAt > @now ORDER BY CreatedAt DESC LIMIT 1;";
        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("O"));
        
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadToken(reader);
        }
        return null;
    }
    
    public async Task<int> CreateAsync(PasswordResetToken token)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO PasswordResetTokens (UserId, TokenHash, ExpiresAt, CreatedAt)
            VALUES (@userId, @hash, @expires, @created);
            SELECT last_insert_rowid();";
        
        command.Parameters.AddWithValue("@userId", token.UserId);
        command.Parameters.AddWithValue("@hash", token.TokenHash);
        command.Parameters.AddWithValue("@expires", token.ExpiresAt.ToString("O"));
        command.Parameters.AddWithValue("@created", token.CreatedAt.ToString("O"));
        
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
    
    public async Task<bool> MarkAsUsedAsync(int id)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE PasswordResetTokens SET UsedAt = @used WHERE Id = @id;";
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@used", DateTime.UtcNow.ToString("O"));
        
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }
    
    public async Task<bool> DeleteExpiredAsync()
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM PasswordResetTokens WHERE ExpiresAt < @now;";
        command.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("O"));
        
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }
    
    public async Task<bool> DeleteByUserIdAsync(int userId)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM PasswordResetTokens WHERE UserId = @userId;";
        command.Parameters.AddWithValue("@userId", userId);
        
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }
    
    private static PasswordResetToken ReadToken(SqliteDataReader reader)
    {
        return new PasswordResetToken
        {
            Id = reader.GetInt32(0),
            UserId = reader.GetInt32(1),
            TokenHash = (byte[])reader.GetValue(2),
            ExpiresAt = DateTime.Parse(reader.GetString(3)),
            UsedAt = reader.IsDBNull(4) ? null : DateTime.Parse(reader.GetString(4)),
            CreatedAt = DateTime.Parse(reader.GetString(5))
        };
    }
}