using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Planilla_RRHH.Data;

public sealed class SqliteConnectionFactory : IDisposable
{
    private readonly string _connectionString;
    private readonly ILogger<SqliteConnectionFactory>? _logger;
    private SqliteConnection? _sharedConnection;
    private bool _disposed;
    
    public SqliteConnectionFactory(string databasePath, ILogger<SqliteConnectionFactory>? logger = null)
    {
        _logger = logger;
        var directory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared,
            ForeignKeys = true
        }.ToString();
    }
    
    public SqliteConnection CreateConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        EnableForeignKeys(connection);
        return connection;
    }
    
    public SqliteConnection GetSharedConnection()
    {
        if (_sharedConnection == null || _sharedConnection.State != System.Data.ConnectionState.Open)
        {
            _sharedConnection = CreateConnection();
        }
        return _sharedConnection;
    }
    
    private static void EnableForeignKeys(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys = ON;";
        command.ExecuteNonQuery();
        
        command.CommandText = "PRAGMA journal_mode = WAL;";
        command.ExecuteNonQuery();
    }
    
    public void Dispose()
    {
        if (!_disposed)
        {
            _sharedConnection?.Dispose();
            _sharedConnection = null;
            _disposed = true;
        }
    }
}