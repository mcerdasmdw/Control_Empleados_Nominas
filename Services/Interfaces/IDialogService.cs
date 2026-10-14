using Avalonia.Controls;

namespace Planilla_RRHH.Services.Interfaces;

public interface IDialogService
{
    void SetOwner(Window owner);
    Task<bool> ShowConfirmAsync(string title, string message, string confirmText = "Confirmar", string cancelText = "Cancelar");
    Task ShowInfoAsync(string title, string message);
    Task ShowErrorAsync(string title, string message);
    Task ShowWarningAsync(string title, string message);
    Task<string?> ShowInputAsync(string title, string message, string placeholder = "");
    Task<string?> ShowFileOpenAsync(string title, string? filter = null);
    Task<string?> ShowFileSaveAsync(string title, string? filter = null, string? defaultName = null);
    Task<string?> ShowFolderSelectAsync(string title);
}