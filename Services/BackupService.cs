using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Planilla_RRHH.Data;
using Planilla_RRHH.Services.Interfaces;
using System.IO;

namespace Planilla_RRHH.Services;

public sealed class BackupService : IBackupService
{
    private readonly Database _database;
    private readonly ILogger<BackupService>? _logger;
    private readonly string _currentDbPath;
    
    public BackupService(Database database, ILogger<BackupService>? logger = null)
    {
        _database = database;
        _logger = logger;
        _currentDbPath = GetCurrentDatabasePath();
    }
    
    public async Task<(bool success, string? error, string? backupPath)> CreateBackupAsync(string destinationPath)
    {
        try
        {
            var directory = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // Use file copy with SQLite backup API for consistent backup
            using var sourceConn = _database.CreateConnection();
            using var destConn = new SqliteConnection($"Data Source={destinationPath};");
            await destConn.OpenAsync();
            
            // Enable WAL mode on destination
            using (var cmd = destConn.CreateCommand())
            {
                cmd.CommandText = "PRAGMA journal_mode = WAL;";
                await cmd.ExecuteNonQueryAsync();
            }
            
            // Perform backup using SQLite's backup API
            sourceConn.BackupDatabase(destConn);
            
            // Verify backup integrity
            var validateResult = await ValidateBackupAsync(destinationPath);
            if (!validateResult.isValid)
            {
                File.Delete(destinationPath);
                return (false, $"Backup verification failed: {validateResult.error}", null);
            }
            
            var fileInfo = new FileInfo(destinationPath);
            _logger?.LogInformation("Backup created: {Path} ({Size} bytes)", destinationPath, fileInfo.Length);
            
            return (true, null, destinationPath);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to create backup at {Path}", destinationPath);
            if (File.Exists(destinationPath))
            {
                try { File.Delete(destinationPath); } catch { }
            }
            return (false, $"Error al crear backup: {ex.Message}", null);
        }
    }
    
    public async Task<(bool success, string? error)> RestoreBackupAsync(string backupPath)
    {
        if (!File.Exists(backupPath))
            return (false, "El archivo de backup no existe");
        
        var validateResult = await ValidateBackupAsync(backupPath);
        if (!validateResult.isValid)
            return (false, $"Backup inválido: {validateResult.error}");
        
        try
        {
            if (string.IsNullOrEmpty(_currentDbPath))
                return (false, "No se pudo determinar la ruta de la base de datos actual");
            
            // Create preventive backup of current database
            var preventiveBackup = _currentDbPath + $".pre-restore-{DateTime.UtcNow:yyyyMMddHHmmss}.bak";
            File.Copy(_currentDbPath, preventiveBackup, true);
            _logger?.LogInformation("Preventive backup created: {Path}", preventiveBackup);
            
            try
            {
                // Replace database file
                File.Copy(backupPath, _currentDbPath, true);
                
                // Verify restored database
                var restoredResult = await ValidateBackupAsync(_currentDbPath);
                if (!restoredResult.isValid)
                {
                    // Try to restore preventive backup
                    File.Copy(preventiveBackup, _currentDbPath, true);
                    return (false, $"Restauración fallida, base de datos original restaurada: {restoredResult.error}");
                }
                
                _logger?.LogInformation("Database restored from: {Path}", backupPath);
                return (true, null);
            }
            catch (Exception)
            {
                // Try to restore preventive backup
                try { File.Copy(preventiveBackup, _currentDbPath, true); } catch { }
                throw;
            }
            finally
            {
                // Clean up preventive backup
                try { File.Delete(preventiveBackup); } catch { }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to restore backup from {Path}", backupPath);
            return (false, $"Error al restaurar backup: {ex.Message}");
        }
    }
    
    public async Task<(bool success, string? error, bool isValid)> ValidateBackupAsync(string backupPath)
    {
        if (!File.Exists(backupPath))
            return (false, "Archivo no encontrado", false);
        
        try
        {
            using var conn = new SqliteConnection($"Data Source={backupPath};Mode=ReadOnly;");
            await conn.OpenAsync();
            
            // Check if it's a valid SQLite database
            using var checkCmd = conn.CreateCommand();
            checkCmd.CommandText = "SELECT 1 FROM sqlite_master WHERE type='table' AND name='Users';";
            var result = await checkCmd.ExecuteScalarAsync();
            
            if (result == null)
                return (false, "No es una base de datos válida (falta tabla Users)", false);
            
            // Check schema version
            checkCmd.CommandText = "SELECT MAX(Version) FROM SchemaVersion;";
            var versionResult = await checkCmd.ExecuteScalarAsync();
            var version = versionResult != null && versionResult != DBNull.Value ? Convert.ToInt32(versionResult) : 0;
            
            if (version < 1)
                return (false, "Versión de esquema incompatible", false);
            
            // Check foreign keys
            checkCmd.CommandText = "PRAGMA foreign_key_check;";
            using var fkReader = await checkCmd.ExecuteReaderAsync();
            if (await fkReader.ReadAsync())
                return (false, "La base de datos tiene restricciones de clave foránea violadas", false);
            
            // Quick integrity check
            checkCmd.CommandText = "PRAGMA quick_check;";
            var integrityResult = await checkCmd.ExecuteScalarAsync();
            if (integrityResult?.ToString() != "ok")
                return (false, $"Comprobación de integridad fallida: {integrityResult}", false);
            
            return (true, null, true);
        }
        catch (Exception ex)
        {
            return (false, $"Error al validar: {ex.Message}", false);
        }
    }
    
    public Task<IReadOnlyList<BackupInfo>> GetBackupHistoryAsync()
    {
        var defaultBackupDir = GetDefaultBackupDirectory();
        var backups = new List<BackupInfo>();
        
        if (Directory.Exists(defaultBackupDir))
        {
            var files = Directory.GetFiles(defaultBackupDir, "*.db")
                .Union(Directory.GetFiles(defaultBackupDir, "*.bak"))
                .Union(Directory.GetFiles(defaultBackupDir, "*.sqlite*"));
            
            foreach (var file in files)
            {
                var info = new FileInfo(file);
                backups.Add(new BackupInfo
                {
                    FilePath = file,
                    FileName = info.Name,
                    SizeBytes = info.Length,
                    CreatedAt = info.CreationTimeUtc
                });
            }
        }
        
        return Task.FromResult<IReadOnlyList<BackupInfo>>(backups.OrderByDescending(b => b.CreatedAt).ToList());
    }
    
    private static string GetCurrentDatabasePath()
    {
        string baseDir;
        if (OperatingSystem.IsWindows())
        {
            baseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        else
        {
            baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
        
        var appDir = Path.Combine(baseDir, "Planilla-RRHH");
        return Path.Combine(appDir, "planilla.db");
    }
    
    private static string GetDefaultBackupDirectory()
    {
        var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        return Path.Combine(baseDir, "Planilla-RRHH", "Backups");
    }
}