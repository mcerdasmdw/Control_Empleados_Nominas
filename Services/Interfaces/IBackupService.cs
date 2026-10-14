namespace Planilla_RRHH.Services.Interfaces;

public interface IBackupService
{
    Task<(bool success, string? error, string? backupPath)> CreateBackupAsync(string destinationPath);
    Task<(bool success, string? error)> RestoreBackupAsync(string backupPath);
    Task<(bool success, string? error, bool isValid)> ValidateBackupAsync(string backupPath);
    Task<IReadOnlyList<BackupInfo>> GetBackupHistoryAsync();
}

public class BackupInfo
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime CreatedAt { get; set; }
}