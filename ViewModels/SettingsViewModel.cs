using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Planilla_RRHH.Models;
using Planilla_RRHH.Services.Interfaces;

namespace Planilla_RRHH.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly IDialogService _dialogService;
    
    [ObservableProperty]
    private string _commercialName = string.Empty;
    
    [ObservableProperty]
    private string _legalId = string.Empty;
    
    [ObservableProperty]
    private string _phone = string.Empty;
    
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _address = string.Empty;
    
    [ObservableProperty]
    private string _currency = "CRC";
    
    [ObservableProperty]
    private string _logoPath = string.Empty;
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _statusMessage = string.Empty;
    
    public SettingsViewModel(ISettingsService settingsService, IDialogService dialogService)
    {
        _settingsService = settingsService;
        _dialogService = dialogService;
    }
    
    public void LoadSettings()
    {
        _ = LoadSettingsAsync();
    }
    
    private async Task LoadSettingsAsync()
    {
        IsLoading = true;
        StatusMessage = "Cargando...";
        
        try
        {
            var settings = await _settingsService.GetAsync();
            CommercialName = settings.CommercialName ?? "";
            LegalId = settings.LegalId ?? "";
            Phone = settings.Phone ?? "";
            Email = settings.Email ?? "";
            Address = settings.Address ?? "";
            Currency = settings.Currency;
            LogoPath = settings.LogoPath ?? "";
            StatusMessage = "Listo";
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
    private async Task SaveAsync()
    {
        IsLoading = true;
        StatusMessage = "Guardando...";
        ErrorMessage = string.Empty;
        
        try
        {
            var settings = new RestaurantSettings
            {
                CommercialName = string.IsNullOrWhiteSpace(CommercialName) ? null : CommercialName.Trim(),
                LegalId = string.IsNullOrWhiteSpace(LegalId) ? null : LegalId.Trim(),
                Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim(),
                Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                Address = string.IsNullOrWhiteSpace(Address) ? null : Address.Trim(),
                Currency = Currency,
                LogoPath = string.IsNullOrWhiteSpace(LogoPath) ? null : LogoPath.Trim()
            };
            
            var (success, error) = await _settingsService.UpdateAsync(settings);
            if (success)
            {
                StatusMessage = "Configuración guardada";
                await _dialogService.ShowInfoAsync("Éxito", "La configuración se ha guardado correctamente");
            }
            else
            {
                ErrorMessage = error ?? "Error al guardar";
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
    private async Task SelectLogoAsync()
    {
        var file = await _dialogService.ShowFileOpenAsync("Seleccionar logotipo", "Imágenes (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg");
        if (!string.IsNullOrEmpty(file))
        {
            LogoPath = file;
        }
    }
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
}