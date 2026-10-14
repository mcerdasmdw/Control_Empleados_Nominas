using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Planilla_RRHH.Models;
using Planilla_RRHH.Models.Enums;
using Planilla_RRHH.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Planilla_RRHH.ViewModels;

public partial class UsersViewModel : ViewModelBase
{
    private readonly IUserService _userService;
    private readonly IDialogService _dialogService;
    private readonly IPermissionService _permissionService;
    private readonly Func<User?, Task> _onEditRequested;
    
    [ObservableProperty]
    private ObservableCollection<UserItemViewModel> _users = new();
    
    [ObservableProperty]
    private bool _isLoading;
    
    public UsersViewModel(IUserService userService, IDialogService dialogService, IPermissionService permissionService, Func<User?, Task> onEditRequested)
    {
        _userService = userService;
        _dialogService = dialogService;
        _permissionService = permissionService;
        _onEditRequested = onEditRequested;
    }
    
    public bool CanManageUsers => _permissionService.CanManageUsers();
    
    public void LoadUsers()
    {
        _ = LoadUsersAsync();
    }
    
    private async Task LoadUsersAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _userService.GetAllAsync();
            Users.Clear();
            
            foreach (var user in list)
            {
                var role = await _userService.GetRoleAsync(user.Id);
                Users.Add(new UserItemViewModel(user, role));
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Error", $"Error al cargar usuarios: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task AddUserAsync()
    {
        await _onEditRequested(null);
    }
    
    [RelayCommand]
    private async Task EditUserAsync(UserItemViewModel? item)
    {
        if (item == null) return;
        
        var user = await _userService.GetByIdAsync(item.Id);
        if (user != null)
        {
            await _onEditRequested(user);
        }
    }
    
    [RelayCommand]
    private async Task ToggleActiveAsync(UserItemViewModel? item)
    {
        if (item == null) return;
        
        var user = await _userService.GetByIdAsync(item.Id);
        if (user != null)
        {
            user.IsActive = !user.IsActive;
            var (success, error) = await _userService.UpdateAsync(user);
            if (success)
            {
                LoadUsers();
            }
            else
            {
                await _dialogService.ShowErrorAsync("Error", error ?? "No se pudo actualizar el usuario");
            }
        }
    }
    
    [RelayCommand]
    private async Task ResetPasswordAsync(UserItemViewModel? item)
    {
        if (item == null) return;
        
        var newPassword = await _dialogService.ShowInputAsync("Restablecer contraseña", "Nueva contraseña (mín. 8 caracteres, mayúscula, minúscula, número):");
        if (string.IsNullOrWhiteSpace(newPassword)) return;
        
        var (success, error) = await _userService.ChangePasswordAsync(item.Id, newPassword);
        if (success)
        {
            await _dialogService.ShowInfoAsync("Éxito", "Contraseña actualizada");
        }
        else
        {
            await _dialogService.ShowErrorAsync("Error", error ?? "No se pudo cambiar la contraseña");
        }
    }
}

public partial class UserItemViewModel : ObservableObject
{
    public int Id { get; }
    public string Username { get; }
    public string Email { get; }
    public bool IsActive { get; }
    public string RoleName { get; }
    
    public UserItemViewModel(User user, Role? role)
    {
        Id = user.Id;
        Username = user.Username;
        Email = user.Email ?? "";
        IsActive = user.IsActive;
        RoleName = role?.Name ?? "Sin rol";
    }
}