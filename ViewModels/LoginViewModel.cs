using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Planilla_RRHH.Services.Interfaces;
using Planilla_RRHH.Security;

namespace Planilla_RRHH.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly IDialogService _dialogService;
    private readonly SessionContext _session;
    private readonly Action _onLoginSuccess;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanLogin))]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _username = string.Empty;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanLogin))]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _password = string.Empty;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanLogin))]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private bool _isLoading;
    
    public LoginViewModel(IAuthService authService, IDialogService dialogService, SessionContext session, Action onLoginSuccess)
    {
        _authService = authService;
        _dialogService = dialogService;
        _session = session;
        _onLoginSuccess = onLoginSuccess;
    }
    
    public bool CanLogin => !IsLoading && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
    
    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        
        try
        {
            var (success, error, user) = await _authService.LoginAsync(Username, Password);
            if (success)
            {
                _onLoginSuccess();
            }
            else
            {
                ErrorMessage = error ?? "Error desconocido";
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
    private async Task ForgotPasswordAsync()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            await _dialogService.ShowWarningAsync("Recuperar contraseña", "Ingrese su nombre de usuario primero");
            return;
        }
        
        var (success, error) = await _authService.RequestPasswordResetAsync(Username);
        if (success)
        {
            await _dialogService.ShowInfoAsync("Recuperar contraseña", "Si el usuario existe, se ha generado un token de recuperación. Revise la consola para ver el token (en producción se enviaría por email).");
        }
        else
        {
            await _dialogService.ShowErrorAsync("Error", error ?? "No se pudo procesar la solicitud");
        }
    }
}
