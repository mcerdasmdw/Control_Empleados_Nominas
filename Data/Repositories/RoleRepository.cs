using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Planilla_RRHH.Models;
using Planilla_RRHH.Models.Enums;
using Planilla_RRHH.Data;

namespace Planilla_RRHH.Data.Repositories;

public sealed class RoleRepository : IRoleRepository
{
    private readonly Database _database;
    private readonly ILogger<RoleRepository>? _logger;
    
    public RoleRepository(Database database, ILogger<RoleRepository>? logger = null)
    {
        _database = database;
        _logger = logger;
    }
    
    public async Task<Role?> GetByIdAsync(int id)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Description, Permissions FROM Roles WHERE Id = @id;";
        command.Parameters.AddWithValue("@id", id);
        
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadRole(reader);
        }
        return null;
    }
    
    public async Task<Role?> GetByNameAsync(string name)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Description, Permissions FROM Roles WHERE Name = @name;";
        command.Parameters.AddWithValue("@name", name);
        
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadRole(reader);
        }
        return null;
    }
    
    public async Task<IReadOnlyList<Role>> GetAllAsync()
    {
        var roles = new List<Role>();
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Description, Permissions FROM Roles ORDER BY Name;";
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            roles.Add(ReadRole(reader));
        }
        return roles;
    }
    
    public async Task<int> CreateAsync(Role role)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Roles (Name, Description, Permissions)
            VALUES (@name, @desc, @perms);
            SELECT last_insert_rowid();";
        
        command.Parameters.AddWithValue("@name", role.Name);
        command.Parameters.AddWithValue("@desc", role.Description ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@perms", (int)role.Permissions);
        
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
    
    public async Task<bool> UpdateAsync(Role role)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Roles SET Name = @name, Description = @desc, Permissions = @perms WHERE Id = @id;";
        
        command.Parameters.AddWithValue("@id", role.Id);
        command.Parameters.AddWithValue("@name", role.Name);
        command.Parameters.AddWithValue("@desc", role.Description ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@perms", (int)role.Permissions);
        
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Roles WHERE Id = @id;";
        command.Parameters.AddWithValue("@id", id);
        
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
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