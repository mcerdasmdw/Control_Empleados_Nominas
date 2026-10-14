using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Planilla_RRHH.Models.Enums;
using Planilla_RRHH.Security;
using Planilla_RRHH.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Planilla_RRHH.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly IPermissionService _permissionService;
    private readonly IDialogService _dialogService;
    private readonly SessionContext _session;
    
    [ObservableProperty]
    private ViewModelBase? _currentView;
    
    [ObservableProperty]
    private string _windowTitle = "Planilla-RRHH";
    
    [ObservableProperty]
    private bool _isSidebarExpanded = true;
    
    public ObservableCollection<NavigationItem> NavigationItems { get; } = new();
    
    // Expose session for binding
    public SessionContext Session => _session;
    
    public MainViewModel(
        IAuthService authService,
        IPermissionService permissionService,
        IDialogService dialogService,
        SessionContext session,
        DashboardViewModel dashboardViewModel,
        EmployeeListViewModel employeeListViewModel,
        SettingsViewModel settingsViewModel,
        UsersViewModel usersViewModel,
        BackupViewModel backupViewModel)
    {
        _authService = authService;
        _permissionService = permissionService;
        _dialogService = dialogService;
        _session = session;
        
        // Store view models for navigation
        _dashboardViewModel = dashboardViewModel;
        _employeeListViewModel = employeeListViewModel;
        _settingsViewModel = settingsViewModel;
        _usersViewModel = usersViewModel;
        _backupViewModel = backupViewModel;
        
        // Subscribe to session changes
        _session.PropertyChanged += (s, e) => UpdateNavigation();
    }
    
    private readonly DashboardViewModel _dashboardViewModel;
    private readonly EmployeeListViewModel _employeeListViewModel;
    private readonly SettingsViewModel _settingsViewModel;
    private readonly UsersViewModel _usersViewModel;
    private readonly BackupViewModel _backupViewModel;
    
    public void Initialize()
    {
        UpdateNavigation();
        NavigateToDashboard();
    }
    
    private void UpdateNavigation()
    {
        NavigationItems.Clear();
        
        NavigationItems.Add(new NavigationItem("Dashboard", "Home", () => NavigateToDashboard(), true));
        NavigationItems.Add(new NavigationItem("Empleados", "Users", () => NavigateToEmployees(), _permissionService.CanAccessEmployeeManagement()));
        
        if (_permissionService.CanManageSettings())
            NavigationItems.Add(new NavigationItem("Configuración", "Settings", () => NavigateToSettings(), true));
        
        if (_permissionService.CanManageUsers())
            NavigationItems.Add(new NavigationItem("Usuarios", "UserCog", () => NavigateToUsers(), true));
        
        if (_permissionService.CanManageBackups())
            NavigationItems.Add(new NavigationItem("Backups", "Database", () => NavigateToBackups(), true));
        
        // Future module - always visible but disabled
        NavigationItems.Add(new NavigationItem("Nómina (próximamente)", "Currency", () => { }, false));
    }
    
    [RelayCommand]
    private void NavigateToDashboard()
    {
        _dashboardViewModel.LoadData();
        CurrentView = _dashboardViewModel;
        WindowTitle = "Planilla-RRHH - Dashboard";
    }
    
    [RelayCommand]
    private void NavigateToEmployees()
    {
        _employeeListViewModel.LoadEmployees();
        CurrentView = _employeeListViewModel;
        WindowTitle = "Planilla-RRHH - Empleados";
    }
    
    [RelayCommand]
    private void NavigateToSettings()
    {
        _settingsViewModel.LoadSettings();
        CurrentView = _settingsViewModel;
        WindowTitle = "Planilla-RRHH - Configuración";
    }
    
    [RelayCommand]
    private void NavigateToUsers()
    {
        _usersViewModel.LoadUsers();
        CurrentView = _usersViewModel;
        WindowTitle = "Planilla-RRHH - Usuarios";
    }
    
    [RelayCommand]
    private void NavigateToBackups()
    {
        _backupViewModel.LoadBackups();
        CurrentView = _backupViewModel;
        WindowTitle = "Planilla-RRHH - Backups";
    }
    
    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirm = await _dialogService.ShowConfirmAsync("Cerrar sesión", "¿Está seguro de que desea cerrar sesión?");
        if (confirm)
        {
            _authService.Logout();
            // Navigation will be handled by App to show login
        }
    }
    
    [RelayCommand]
    private void ToggleSidebar()
    {
        IsSidebarExpanded = !IsSidebarExpanded;
    }
}

public class NavigationItem
{
    public string Title { get; }
    public string Icon { get; }
    public Action Action { get; }
    public bool IsEnabled { get; }
    
    public NavigationItem(string title, string icon, Action action, bool isEnabled)
    {
        Title = title;
        Icon = icon;
        Action = action;
        IsEnabled = isEnabled;
    }
}