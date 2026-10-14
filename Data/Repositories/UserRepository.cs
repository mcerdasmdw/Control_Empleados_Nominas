using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Planilla_RRHH.Models;
using Planilla_RRHH.Models.Enums;
using Planilla_RRHH.Data;

namespace Planilla_RRHH.Data.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly Database _database;
    private readonly ILogger<UserRepository>? _logger;
    
    public UserRepository(Database database, ILogger<UserRepository>? logger = null)
    {
        _database = database;
        _logger = logger;
    }
    
    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT Id, Username, PasswordHash, PasswordSalt, Argon2Params, Email, IsActive, CreatedAt, UpdatedAt FROM Users WHERE Id = @id;";
        command.Parameters.AddWithValue("@id", id);
        
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadUser(reader);
        }
        return null;
    }
    
    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT Id, Username, PasswordHash, PasswordSalt, Argon2Params, Email, IsActive, CreatedAt, UpdatedAt FROM Users WHERE Username = @username;";
        command.Parameters.AddWithValue("@username", username);
        
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadUser(reader);
        }
        return null;
    }
    
    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        var users = new List<User>();
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT Id, Username, PasswordHash, PasswordSalt, Argon2Params, Email, IsActive, CreatedAt, UpdatedAt FROM Users ORDER BY Username;";
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            users.Add(ReadUser(reader));
        }
        return users;
    }
    
    public async Task<int> CreateAsync(User user)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Users (Username, PasswordHash, PasswordSalt, Argon2Params, Email, IsActive, CreatedAt, UpdatedAt)
            VALUES (@username, @hash, @salt, @params, @email, @active, @created, @updated);
            SELECT last_insert_rowid();";
        
        command.Parameters.AddWithValue("@username", user.Username);
        command.Parameters.AddWithValue("@hash", user.PasswordHash);
        command.Parameters.AddWithValue("@salt", user.PasswordSalt);
        command.Parameters.AddWithValue("@params", user.Argon2Params);
        command.Parameters.AddWithValue("@email", user.Email ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@active", user.IsActive ? 1 : 0);
        command.Parameters.AddWithValue("@created", user.CreatedAt.ToString("O"));
        command.Parameters.AddWithValue("@updated", user.UpdatedAt.ToString("O"));
        
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
    
    public async Task<bool> UpdateAsync(User user)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Users SET 
                Username = @username,
                Email = @email,
                IsActive = @active,
                UpdatedAt = @updated
            WHERE Id = @id;";
        
        command.Parameters.AddWithValue("@id", user.Id);
        command.Parameters.AddWithValue("@username", user.Username);
        command.Parameters.AddWithValue("@email", user.Email ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@active", user.IsActive ? 1 : 0);
        command.Parameters.AddWithValue("@updated", DateTime.UtcNow.ToString("O"));
        
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Users WHERE Id = @id;";
        command.Parameters.AddWithValue("@id", id);
        
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }
    
    public async Task<bool> ExistsByUsernameAsync(string username, int? excludeId = null)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        
        if (excludeId.HasValue)
        {
            command.CommandText = "SELECT COUNT(1) FROM Users WHERE Username = @username AND Id != @excludeId;";
            command.Parameters.AddWithValue("@excludeId", excludeId.Value);
        }
        else
        {
            command.CommandText = "SELECT COUNT(1) FROM Users WHERE Username = @username;";
        }
        command.Parameters.AddWithValue("@username", username);
        
        var count = await command.ExecuteScalarAsync();
        return Convert.ToInt64(count) > 0;
    }
    
    public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        
        if (excludeId.HasValue)
        {
            command.CommandText = "SELECT COUNT(1) FROM Users WHERE Email = @email AND Id != @excludeId;";
            command.Parameters.AddWithValue("@excludeId", excludeId.Value);
        }
        else
        {
            command.CommandText = "SELECT COUNT(1) FROM Users WHERE Email = @email;";
        }
        command.Parameters.AddWithValue("@email", email);
        
        var count = await command.ExecuteScalarAsync();
        return Convert.ToInt64(count) > 0;
    }
    
    public async Task<Role?> GetUserRoleAsync(int userId)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT r.Id, r.Name, r.Description, r.Permissions
            FROM Roles r
            INNER JOIN UserRoles ur ON r.Id = ur.RoleId
            WHERE ur.UserId = @userId;";
        command.Parameters.AddWithValue("@userId", userId);
        
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadRole(reader);
        }
        return null;
    }
    
    public async Task<bool> AssignRoleAsync(int userId, int roleId)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT OR REPLACE INTO UserRoles (UserId, RoleId) VALUES (@userId, @roleId);";
        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@roleId", roleId);
        
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }
    
    public async Task<bool> RemoveRoleAsync(int userId)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM UserRoles WHERE UserId = @userId;";
        command.Parameters.AddWithValue("@userId", userId);
        
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }
    
    private static User ReadUser(SqliteDataReader reader)
    {
        return new User
        {
            Id = reader.GetInt32(0),
            Username = reader.GetString(1),
            PasswordHash = (byte[])reader.GetValue(2),
            PasswordSalt = (byte[])reader.GetValue(3),
            Argon2Params = reader.GetString(4),
            Email = reader.IsDBNull(5) ? null : reader.GetString(5),
            IsActive = reader.GetInt32(6) == 1,
            CreatedAt = DateTime.Parse(reader.GetString(7)),
            UpdatedAt = DateTime.Parse(reader.GetString(8))
        };
    }
    
    private static Role ReadRole(SqliteDataReader reader)
    {
        return new Role
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
            Permissions = (PermissionFlags)reader.GetInt32(3)
        };
    }
}