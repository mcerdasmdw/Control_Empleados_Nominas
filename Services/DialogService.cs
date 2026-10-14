using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Planilla_RRHH.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Planilla_RRHH.Services;

public sealed class DialogService : IDialogService
{
    private Window? _owner;
    
    public void SetOwner(Window owner) => _owner = owner;
    
    public async Task<bool> ShowConfirmAsync(string title, string message, string confirmText = "Confirmar", string cancelText = "Cancelar")
    {
        bool result = false;
        Window? dialog = null;
        
        var cancelBtn = new Button { Content = cancelText, Classes = { "flat" } };
        var confirmBtn = new Button { Content = confirmText, Classes = { "accent" } };
        
        cancelBtn.Click += (s, e) => { result = false; dialog?.Close(); };
        confirmBtn.Click += (s, e) => { result = true; dialog?.Close(); };
        
        dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Spacing = 16,
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Children = { cancelBtn, confirmBtn }
                    }
                }
            }
        };
        
        await dialog.ShowDialog(_owner!);
        return result;
    }
    
    public Task ShowInfoAsync(string title, string message) => ShowMessageAsync(title, message, "Aceptar");
    public Task ShowErrorAsync(string title, string message) => ShowMessageAsync(title, message, "Aceptar");
    public Task ShowWarningAsync(string title, string message) => ShowMessageAsync(title, message, "Aceptar");
    
    private async Task ShowMessageAsync(string title, string message, string buttonText)
    {
        Window? dialog = null;
        var btn = new Button { Content = buttonText, HorizontalAlignment = HorizontalAlignment.Right };
        btn.Click += (s, e) => dialog?.Close();
        
        dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 180,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Spacing = 16,
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap },
                    btn
                }
            }
        };
        
        await dialog.ShowDialog(_owner!);
    }
    
    public async Task<string?> ShowInputAsync(string title, string message, string placeholder = "")
    {
        string? result = null;
        var input = new TextBox { Watermark = placeholder, Width = 360 };
        
        Window? dialog = null;
        var cancelBtn = new Button { Content = "Cancelar", Classes = { "flat" } };
        var acceptBtn = new Button { Content = "Aceptar", Classes = { "accent" } };
        
        cancelBtn.Click += (s, e) => { result = null; dialog?.Close(); };
        acceptBtn.Click += (s, e) => { result = input.Text; dialog?.Close(); };
        
        dialog = new Window
        {
            Title = title,
            Width = 440,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Spacing = 16,
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap },
                    input,
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Children = { cancelBtn, acceptBtn }
                    }
                }
            }
        };
        
        await dialog.ShowDialog(_owner!);
        return result;
    }
    
    public async Task<string?> ShowFileOpenAsync(string title, string? filter = null)
    {
        if (_owner?.StorageProvider == null) return null;
        
        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        };
        
        var result = await _owner.StorageProvider.OpenFilePickerAsync(options);
        return result?.FirstOrDefault()?.Path.LocalPath;
    }
    
    public async Task<string?> ShowFileSaveAsync(string title, string? filter = null, string? defaultName = null)
    {
        if (_owner?.StorageProvider == null) return null;
        
        var options = new FilePickerSaveOptions
        {
            Title = title,
            DefaultExtension = filter?.Split('.').LastOrDefault() ?? "db"
        };
        
        if (!string.IsNullOrEmpty(defaultName))
            options.SuggestedFileName = defaultName;
        
        var result = await _owner.StorageProvider.SaveFilePickerAsync(options);
        return result?.Path.LocalPath;
    }
    
    public async Task<string?> ShowFolderSelectAsync(string title)
    {
        if (_owner?.StorageProvider == null) return null;
        
        var options = new FolderPickerOpenOptions
        {
            Title = title
        };
        
        var result = await _owner.StorageProvider.OpenFolderPickerAsync(options);
        return result?.FirstOrDefault()?.Path.LocalPath;
    }
}