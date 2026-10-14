using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Planilla_RRHH.Models;
using Planilla_RRHH.Models.Enums;
using Planilla_RRHH.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Planilla_RRHH.ViewModels;

public partial class EmployeeListViewModel : ViewModelBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IDialogService _dialogService;
    private readonly IPermissionService _permissionService;
    private readonly Func<Employee?, Task> _onEditRequested;
    
    [ObservableProperty]
    private ObservableCollection<EmployeeItemViewModel> _employees = new();
    
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    [ObservableProperty]
    private bool _includeInactive;
    
    [ObservableProperty]
    private int _currentPage = 1;
    
    [ObservableProperty]
    private int _pageSize = 50;
    
    [ObservableProperty]
    private int _totalCount;
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private EmployeeItemViewModel? _selectedEmployee;
    
    public EmployeeListViewModel(
        IEmployeeService employeeService,
        IDialogService dialogService,
        IPermissionService permissionService,
        Func<Employee?, Task> onEditRequested)
    {
        _employeeService = employeeService;
        _dialogService = dialogService;
        _permissionService = permissionService;
        _onEditRequested = onEditRequested;
    }
    
    public bool CanCreate => _permissionService.CanAccessEmployeeManagement() && _permissionService.HasPermission(PermissionFlags.EmployeeCreate);
    public bool CanEdit => _permissionService.CanAccessEmployeeManagement() && _permissionService.HasPermission(PermissionFlags.EmployeeUpdate);
    public bool CanDelete => _permissionService.CanAccessEmployeeManagement() && _permissionService.HasPermission(PermissionFlags.EmployeeDelete);
    public bool CanViewSalary => _permissionService.CanViewSalaries();
    
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    
    public void LoadEmployees()
    {
        _ = LoadEmployeesAsync();
    }
    
    private async Task LoadEmployeesAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _employeeService.SearchAsync(
                string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                IncludeInactive,
                CurrentPage,
                PageSize);
            
            TotalCount = await _employeeService.GetCountAsync(IncludeInactive);
            
            Employees.Clear();
            foreach (var emp in list)
            {
                Employees.Add(new EmployeeItemViewModel(emp, CanViewSalary));
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Error", $"Error al cargar empleados: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private void Search()
    {
        CurrentPage = 1;
        LoadEmployees();
    }
    
    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
        CurrentPage = 1;
        LoadEmployees();
    }
    
    [RelayCommand]
    private void NextPage()
    {
        if (HasNextPage)
        {
            CurrentPage++;
            LoadEmployees();
        }
    }
    
    [RelayCommand]
    private void PreviousPage()
    {
        if (HasPreviousPage)
        {
            CurrentPage--;
            LoadEmployees();
        }
    }
    
    [RelayCommand]
    private async Task AddEmployeeAsync()
    {
        await _onEditRequested(null);
    }
    
    [RelayCommand]
    private async Task EditEmployeeAsync(EmployeeItemViewModel? item)
    {
        if (item == null) return;
        
        var employee = await _employeeService.GetByIdAsync(item.Id);
        if (employee != null)
        {
            await _onEditRequested(employee);
        }
    }
    
    [RelayCommand]
    private async Task DeleteEmployeeAsync(EmployeeItemViewModel? item)
    {
        if (item == null) return;
        
        var confirm = await _dialogService.ShowConfirmAsync(
            "Desactivar empleado",
            $"¿Desactivar a {item.FullName}? El empleado no se eliminará, solo se marcará como inactivo.",
            "Desactivar", "Cancelar");
        
        if (confirm)
        {
            var (success, error) = await _employeeService.DeleteAsync(item.Id);
            if (success)
            {
                LoadEmployees();
            }
            else
            {
                await _dialogService.ShowErrorAsync("Error", error ?? "No se pudo desactivar el empleado");
            }
        }
    }
    
    partial void OnSearchTextChanged(string value)
    {
        // Debounce search
        if (string.IsNullOrWhiteSpace(value))
        {
            CurrentPage = 1;
            LoadEmployees();
        }
    }
    
    partial void OnIncludeInactiveChanged(bool value)
    {
        CurrentPage = 1;
        LoadEmployees();
    }
}

public partial class EmployeeItemViewModel : ObservableObject
{
    public int Id { get; }
    public string FullName { get; }
    public string Cedula { get; }
    public string Phone { get; }
    public string Position { get; }
    public EmployeeStatus Status { get; }
    public decimal? BaseSalary { get; }
    public bool CanViewSalary { get; }
    
    public EmployeeItemViewModel(Employee employee, bool canViewSalary)
    {
        Id = employee.Id;
        FullName = employee.FullName;
        Cedula = employee.Cedula;
        Phone = employee.Phone ?? "";
        Position = employee.Position ?? "";
        Status = employee.Status;
        BaseSalary = employee.BaseSalary;
        CanViewSalary = canViewSalary && employee.BaseSalary.HasValue;
    }
    
    public string FormattedSalary => CanViewSalary && BaseSalary.HasValue
        ? $"{BaseSalary.Value:N2} CRC"
        : "—";
    
    public string StatusText => Status == EmployeeStatus.Activo ? "Activo" : "Inactivo";
    public string StatusColor => Status == EmployeeStatus.Activo ? "Green" : "Gray";
}