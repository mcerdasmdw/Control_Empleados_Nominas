using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Planilla_RRHH.Models;
using Planilla_RRHH.Models.Enums;
using Planilla_RRHH.Security;
using System.Text.Json;

namespace Planilla_RRHH.Data;

public sealed class Database
{
    private readonly SqliteConnectionFactory _connectionFactory;
    private readonly ILogger<Database>? _logger;
    private const int CurrentSchemaVersion = 1;
    
    public Database(SqliteConnectionFactory connectionFactory, ILogger<Database>? logger = null)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }
    
    public void Initialize()
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            CreateSchema(connection, transaction);
            RunMigrations(connection, transaction);
            transaction.Commit();
            _logger?.LogInformation("Database initialized successfully");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger?.LogError(ex, "Failed to initialize database");
            throw;
        }
    }
    
    private void CreateSchema(SqliteConnection connection, SqliteTransaction transaction)
    {
        var commands = new[]
        {
            // Users table
            @"CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE COLLATE NOCASE,
                PasswordHash BLOB NOT NULL,
                PasswordSalt BLOB NOT NULL,
                Argon2Params TEXT NOT NULL,
                Email TEXT,
                IsActive INTEGER NOT NULL DEFAULT 1,
                CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
            );",
            
            // Roles table
            @"CREATE TABLE IF NOT EXISTS Roles (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                Description TEXT,
                Permissions INTEGER NOT NULL DEFAULT 0
            );",
            
            // UserRoles table (one role per user)
            @"CREATE TABLE IF NOT EXISTS UserRoles (
                UserId INTEGER NOT NULL,
                RoleId INTEGER NOT NULL,
                PRIMARY KEY (UserId),
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
                FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE RESTRICT
            );",
            
            // Employees table
            @"CREATE TABLE IF NOT EXISTS Employees (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FullName TEXT NOT NULL,
                Cedula TEXT NOT NULL UNIQUE,
                Phone TEXT,
                Email TEXT,
                Address TEXT,
                Position TEXT,
                BaseSalary NUMERIC(18,2),
                SalaryType INTEGER,
                HireDate TEXT,
                Status INTEGER NOT NULL DEFAULT 1,
                BankName TEXT,
                BankAccountType INTEGER,
                BankAccountNumber TEXT,
                BankIBAN TEXT,
                Notes TEXT,
                CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
            );",
            
            // RestaurantSettings table (singleton)
            @"CREATE TABLE IF NOT EXISTS RestaurantSettings (
                Id INTEGER PRIMARY KEY CHECK (Id = 1),
                CommercialName TEXT,
                LegalId TEXT,
                Phone TEXT,
                Email TEXT,
                Address TEXT,
                Currency TEXT NOT NULL DEFAULT 'CRC',
                LogoPath TEXT,
                CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
            );",
            
            // PasswordResetTokens table
            @"CREATE TABLE IF NOT EXISTS PasswordResetTokens (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                TokenHash BLOB NOT NULL,
                ExpiresAt TEXT NOT NULL,
                UsedAt TEXT,
                CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            );",
            
            // SchemaVersion table
            @"CREATE TABLE IF NOT EXISTS SchemaVersion (
                Version INTEGER PRIMARY KEY,
                AppliedAt TEXT NOT NULL DEFAULT (datetime('now')),
                Description TEXT
            );",
            
            // Indexes
            @"CREATE INDEX IF NOT EXISTS IX_Employees_Cedula ON Employees(Cedula);",
            @"CREATE INDEX IF NOT EXISTS IX_Employees_Status ON Employees(Status);",
            @"CREATE INDEX IF NOT EXISTS IX_Employees_FullName ON Employees(FullName);",
            @"CREATE INDEX IF NOT EXISTS IX_Users_Username ON Users(Username);",
            @"CREATE INDEX IF NOT EXISTS IX_PasswordResetTokens_UserId ON PasswordResetTokens(UserId);",
            @"CREATE INDEX IF NOT EXISTS IX_PasswordResetTokens_ExpiresAt ON PasswordResetTokens(ExpiresAt);"
        };
        
        foreach (var sql in commands)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            command.ExecuteNonQuery();
        }
    }
    
    private void RunMigrations(SqliteConnection connection, SqliteTransaction transaction)
    {
        int currentVersion = GetCurrentVersion(connection, transaction);
        
        if (currentVersion < 1)
        {
            ApplyMigration1(connection, transaction);
            SetVersion(connection, transaction, 1, "Initial schema with Users, Roles, Employees, Settings, PasswordResetTokens");
        }
        
        // Future migrations go here
        // if (currentVersion < 2) { ApplyMigration2(...); SetVersion(..., 2, "..."); }
    }
    
    private int GetCurrentVersion(SqliteConnection connection, SqliteTransaction transaction)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COALESCE(MAX(Version), 0) FROM SchemaVersion;";
        command.Transaction = transaction;
        var result = command.ExecuteScalar();
        return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
    }
    
    private void SetVersion(SqliteConnection connection, SqliteTransaction transaction, int version, string description)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT OR REPLACE INTO SchemaVersion (Version, Description) VALUES (@v, @d);";
        command.Transaction = transaction;
        command.Parameters.AddWithValue("@v", version);
        command.Parameters.AddWithValue("@d", description);
        command.ExecuteNonQuery();
    }
    
    private void ApplyMigration1(SqliteConnection connection, SqliteTransaction transaction)
    {
        // Seed default roles
        var roles = new[]
        {
            (1, "Administrador", "Acceso completo al sistema", (int)PermissionFlags.AdminAll),
            (2, "Recursos Humanos", "Gestión de empleados", (int)PermissionFlags.RRHH),
            (3, "Supervisor", "Solo lectura de empleados", (int)PermissionFlags.Supervisor),
            (4, "Contabilidad", "Lectura de empleados y salarios", (int)PermissionFlags.Contabilidad)
        };
        
        foreach (var (id, name, desc, perms) in roles)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT OR IGNORE INTO Roles (Id, Name, Description, Permissions) VALUES (@id, @name, @desc, @perms);";
            cmd.Transaction = transaction;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@desc", desc);
            cmd.Parameters.AddWithValue("@perms", perms);
            cmd.ExecuteNonQuery();
        }
        
        // Seed RestaurantSettings singleton
        using var settingsCmd = connection.CreateCommand();
        settingsCmd.CommandText = @"INSERT OR IGNORE INTO RestaurantSettings (Id, Currency) VALUES (1, 'CRC');";
        settingsCmd.Transaction = transaction;
        settingsCmd.ExecuteNonQuery();
    }
    
    public SqliteConnection CreateConnection() => _connectionFactory.CreateConnection();
}