using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Planilla_RRHH.Models;
using Planilla_RRHH.Models.Enums;
using Planilla_RRHH.Services.Interfaces;
using Planilla_RRHH.Security;
using System.Collections.ObjectModel;

namespace Planilla_RRHH.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IEmployeeService _employeeService;
    private readonly ISettingsService _settingsService;
    private readonly SessionContext _session;
    private readonly IPermissionService _permissionService;
    
    [ObservableProperty]
    private int _totalEmployees;
    
    [ObservableProperty]
    private int _activeEmployees;
    
    [ObservableProperty]
    private int _inactiveEmployees;
    
    [ObservableProperty]
    private ObservableCollection<EmployeeSummary> _recentEmployees = new();
    
    [ObservableProperty]
    private RestaurantSettings? _restaurantSettings;
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _statusMessage = string.Empty;
    
    public DashboardViewModel(
        IEmployeeService employeeService,
        ISettingsService settingsService,
        SessionContext session,
        IPermissionService permissionService)
    {
        _employeeService = employeeService;
        _settingsService = settingsService;
        _session = session;
        _permissionService = permissionService;
    }
    
    public void LoadData()
    {
        _ = LoadDataAsync();
    }
    
    private async Task LoadDataAsync()
    {
        IsLoading = true;
        StatusMessage = "Cargando...";
        
        try
        {
            var total = await _employeeService.GetCountAsync(true);
            var active = await _employeeService.GetCountAsync(false);
            var recent = await _employeeService.GetRecentAsync(10);
            var settings = await _settingsService.GetAsync();
            
            TotalEmployees = total;
            ActiveEmployees = active;
            InactiveEmployees = total - active;
            RecentEmployees.Clear();
            
            foreach (var emp in recent)
            {
                RecentEmployees.Add(new EmployeeSummary
                {
                    Id = emp.Id,
                    FullName = emp.FullName,
                    Cedula = emp.Cedula,
                    Position = emp.Position ?? "",
                    Status = emp.Status
                });
            }
            
            RestaurantSettings = settings;
            StatusMessage = $"Actualizado: {DateTime.Now:HH:mm:ss}";
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
    
    public bool CanViewSalaries => _permissionService.CanViewSalaries();
    public bool CanManageEmployees => _permissionService.CanAccessEmployeeManagement();
}

public class EmployeeSummary
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public EmployeeStatus Status { get; set; }
    
    public string StatusText => Status == EmployeeStatus.Activo ? "Activo" : "Inactivo";
    public string StatusColor => Status == EmployeeStatus.Activo ? "Green" : "Gray";
}