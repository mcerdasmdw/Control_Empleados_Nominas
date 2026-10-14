using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Planilla_RRHH.Models;
using Planilla_RRHH.Data;

namespace Planilla_RRHH.Data.Repositories;

public sealed class SettingsRepository : ISettingsRepository
{
    private readonly Database _database;
    private readonly ILogger<SettingsRepository>? _logger;
    
    public SettingsRepository(Database database, ILogger<SettingsRepository>? logger = null)
    {
        _database = database;
        _logger = logger;
    }
    
    public async Task<RestaurantSettings> GetAsync()
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, CommercialName, LegalId, Phone, Email, Address, Currency, LogoPath, CreatedAt, UpdatedAt FROM RestaurantSettings WHERE Id = 1;";
        
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadSettings(reader);
        }
        
        // Return default if not found
        return new RestaurantSettings();
    }
    
    public async Task<bool> UpdateAsync(RestaurantSettings settings)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO RestaurantSettings (Id, CommercialName, LegalId, Phone, Email, Address, Currency, LogoPath, CreatedAt, UpdatedAt)
            VALUES (1, @commercialName, @legalId, @phone, @email, @address, @currency, @logoPath, @created, @updated)
            ON CONFLICT(Id) DO UPDATE SET
                CommercialName = @commercialName,
                LegalId = @legalId,
                Phone = @phone,
                Email = @email,
                Address = @address,
                Currency = @currency,
                LogoPath = @logoPath,
                UpdatedAt = @updated;";
        
        command.Parameters.AddWithValue("@commercialName", settings.CommercialName ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@legalId", settings.LegalId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@phone", settings.Phone ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@email", settings.Email ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@address", settings.Address ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@currency", settings.Currency);
        command.Parameters.AddWithValue("@logoPath", settings.LogoPath ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@created", settings.CreatedAt.ToString("O"));
        command.Parameters.AddWithValue("@updated", DateTime.UtcNow.ToString("O"));
        
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }
    
    private static RestaurantSettings ReadSettings(SqliteDataReader reader)
    {
        return new RestaurantSettings
        {
            Id = reader.GetInt32(0),
            CommercialName = reader.IsDBNull(1) ? null : reader.GetString(1),
            LegalId = reader.IsDBNull(2) ? null : reader.GetString(2),
            Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
            Email = reader.IsDBNull(4) ? null : reader.GetString(4),
            Address = reader.IsDBNull(5) ? null : reader.GetString(5),
            Currency = reader.GetString(6),
            LogoPath = reader.IsDBNull(7) ? null : reader.GetString(7),
            CreatedAt = DateTime.Parse(reader.GetString(8)),
            UpdatedAt = DateTime.Parse(reader.GetString(9))
        };
    }
}