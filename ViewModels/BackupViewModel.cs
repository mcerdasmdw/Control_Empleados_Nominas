using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Planilla_RRHH.Models;
using Planilla_RRHH.Services.Interfaces;
using System.Collections.ObjectModel;
using System.IO;

namespace Planilla_RRHH.ViewModels;

public partial class BackupViewModel : ViewModelBase
{
    private readonly IBackupService _backupService;
    private readonly IDialogService _dialogService;
    
    [ObservableProperty]
    private ObservableCollection<BackupInfoViewModel> _backups = new();
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _statusMessage = string.Empty;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    public BackupViewModel(IBackupService backupService, IDialogService dialogService)
    {
        _backupService = backupService;
        _dialogService = dialogService;
    }
    
    public void LoadBackups()
    {
        _ = LoadBackupsAsync();
    }
    
    [RelayCommand]
    private void LoadBackupsCommand()
    {
        _ = LoadBackupsAsync();
    }
    
    private async Task LoadBackupsAsync()
    {
        IsLoading = true;
        StatusMessage = "Cargando historial...";
        
        try
        {
            var list = await _backupService.GetBackupHistoryAsync();
            Backups.Clear();
            foreach (var b in list)
            {
                Backups.Add(new BackupInfoViewModel(b));
            }
            StatusMessage = $"{Backups.Count} backups encontrados";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task CreateBackupAsync()
    {
        var defaultDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Planilla-RRHH", "Backups");
        
        Directory.CreateDirectory(defaultDir);
        
        var fileName = $"planilla-backup-{DateTime.Now:yyyyMMdd-HHmmss}.db";
        var path = await _dialogService.ShowFileSaveAsync(
            "Guardar backup",
            "Base de datos SQLite (*.db)|*.db",
            fileName);
        
        if (string.IsNullOrEmpty(path)) return;
        
        IsLoading = true;
        StatusMessage = "Creando backup...";
        ErrorMessage = string.Empty;
        
        try
        {
            var (success, error, backupPath) = await _backupService.CreateBackupAsync(path);
            if (success)
            {
                StatusMessage = "Backup creado correctamente";
                await _dialogService.ShowInfoAsync("Éxito", $"Backup creado en:\n{backupPath}");
                LoadBackups();
            }
            else
            {
                ErrorMessage = error ?? "Error al crear backup";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task RestoreBackupAsync()
    {
        var path = await _dialogService.ShowFileOpenAsync("Seleccionar backup para restaurar", "Base de datos SQLite (*.db)|*.db");
        if (string.IsNullOrEmpty(path)) return;
        
        // Validate first
        var (valid, validateError, isValid) = await _backupService.ValidateBackupAsync(path);
        if (!isValid)
        {
            await _dialogService.ShowErrorAsync("Backup inválido", validateError ?? "El archivo no es un backup válido");
            return;
        }
        
        var confirm = await _dialogService.ShowConfirmAsync(
            "Restaurar backup",
            "ADVERTENCIA: Esto reemplazará COMPLETAMENTE la base de datos actual (usuarios, empleados, configuración).\n\n¿Está seguro de que desea continuar?",
            "Restaurar", "Cancelar");
        
        if (!confirm) return;
        
        var confirm2 = await _dialogService.ShowConfirmAsync(
            "Confirmación final",
            "Esta acción NO se puede deshacer. Se creará un backup preventivo antes de restaurar.\n\n¿Confirmar restauración?",
            "Sí, restaurar", "No, cancelar");
        
        if (!confirm2) return;
        
        IsLoading = true;
        StatusMessage = "Restaurando...";
        ErrorMessage = string.Empty;
        
        try
        {
            var (success, error) = await _backupService.RestoreBackupAsync(path);
            if (success)
            {
                StatusMessage = "Base de datos restaurada - Reinicie la aplicación";
                await _dialogService.ShowInfoAsync("Restauración completada", "La base de datos ha sido restaurada. Debe reiniciar la aplicación para que los cambios surtan efecto.");
            }
            else
            {
                ErrorMessage = error ?? "Error al restaurar";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task ValidateBackupAsync(BackupInfoViewModel? item)
    {
        if (item == null) return;
        
        var (success, error, isValid) = await _backupService.ValidateBackupAsync(item.FilePath);
        if (isValid)
        {
            await _dialogService.ShowInfoAsync("Validación", "El backup es válido y compatible");
        }
        else
        {
            await _dialogService.ShowErrorAsync("Validación fallida", error ?? "El backup no es válido");
        }
    }
}

public partial class BackupInfoViewModel : ObservableObject
{
    public string FilePath { get; }
    public string FileName { get; }
    public long SizeBytes { get; }
    public DateTime CreatedAt { get; }
    
    public string FormattedSize => SizeBytes switch
    {
        < 1024 => $"{SizeBytes} B",
        < 1024 * 1024 => $"{SizeBytes / 1024.0:F1} KB",
        _ => $"{SizeBytes / (1024.0 * 1024.0):F1} MB"
    };
    
    public BackupInfoViewModel(Planilla_RRHH.Services.Interfaces.BackupInfo info)
    {
        FilePath = info.FilePath;
        FileName = info.FileName;
        SizeBytes = info.SizeBytes;
        CreatedAt = info.CreatedAt;
    }
}