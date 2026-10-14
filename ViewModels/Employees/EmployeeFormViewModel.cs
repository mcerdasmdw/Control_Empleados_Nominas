using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Planilla_RRHH.Models;
using Planilla_RRHH.Models.Enums;
using Planilla_RRHH.Services.Interfaces;

namespace Planilla_RRHH.ViewModels.Employees;

public partial class EmployeeFormViewModel : ViewModelBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IDialogService _dialogService;
    private readonly Action _onClose;
    private Employee? _originalEmployee;
    
    [ObservableProperty]
    private string _title = "Nuevo empleado";
    
    [ObservableProperty]
    private string _fullName = string.Empty;
    
    [ObservableProperty]
    private string _cedula = string.Empty;
    
    [ObservableProperty]
    private string _phone = string.Empty;
    
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _address = string.Empty;
    
    [ObservableProperty]
    private string _position = string.Empty;
    
    [ObservableProperty]
    private decimal? _baseSalary;
    
    [ObservableProperty]
    private SalaryType? _salaryType;
    
    [ObservableProperty]
    private DateTime? _hireDate;
    
    [ObservableProperty]
    private EmployeeStatus _status = EmployeeStatus.Activo;
    
    [ObservableProperty]
    private string _bankName = string.Empty;
    
    [ObservableProperty]
    private BankAccountType? _bankAccountType;
    
    [ObservableProperty]
    private string _bankAccountNumber = string.Empty;
    
    [ObservableProperty]
    private string _bankIBAN = string.Empty;
    
    [ObservableProperty]
    private string _notes = string.Empty;
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    public IEnumerable<SalaryType> SalaryTypes => Enum.GetValues<SalaryType>();
    public IEnumerable<EmployeeStatus> EmployeeStatuses => Enum.GetValues<EmployeeStatus>();
    public IEnumerable<BankAccountType> BankAccountTypes => Enum.GetValues<BankAccountType>();
    public bool IsEditing => _originalEmployee != null;
    
    public EmployeeFormViewModel(IEmployeeService employeeService, IDialogService dialogService, Action onClose)
    {
        _employeeService = employeeService;
        _dialogService = dialogService;
        _onClose = onClose;
        
        HireDate = DateTime.Today;
    }
    
    public void Initialize(Employee? employee)
    {
        if (employee != null)
        {
            _originalEmployee = employee;
            Title = "Editar empleado";
            
            FullName = employee.FullName;
            Cedula = employee.Cedula;
            Phone = employee.Phone ?? "";
            Email = employee.Email ?? "";
            Address = employee.Address ?? "";
            Position = employee.Position ?? "";
            BaseSalary = employee.BaseSalary;
            SalaryType = employee.SalaryType;
            HireDate = employee.HireDate;
            Status = employee.Status;
            BankName = employee.BankName ?? "";
            BankAccountType = employee.BankAccountType;
            BankAccountNumber = employee.BankAccountNumber ?? "";
            BankIBAN = employee.BankIBAN ?? "";
            Notes = employee.Notes ?? "";
        }
        else
        {
            _originalEmployee = null;
            Title = "Nuevo empleado";
            ResetForm();
        }
    }
    
    private void ResetForm()
    {
        FullName = string.Empty;
        Cedula = string.Empty;
        Phone = string.Empty;
        Email = string.Empty;
        Address = string.Empty;
        Position = string.Empty;
        BaseSalary = null;
        SalaryType = null;
        HireDate = DateTime.Today;
        Status = EmployeeStatus.Activo;
        BankName = string.Empty;
        BankAccountType = null;
        BankAccountNumber = string.Empty;
        BankIBAN = string.Empty;
        Notes = string.Empty;
    }
    
    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = string.Empty;
        IsLoading = true;
        
        try
        {
            var employee = new Employee
            {
                FullName = FullName.Trim(),
                Cedula = Cedula.Trim(),
                Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim(),
                Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                Address = string.IsNullOrWhiteSpace(Address) ? null : Address.Trim(),
                Position = string.IsNullOrWhiteSpace(Position) ? null : Position.Trim(),
                BaseSalary = BaseSalary,
                SalaryType = SalaryType,
                HireDate = HireDate,
                Status = Status,
                BankName = string.IsNullOrWhiteSpace(BankName) ? null : BankName.Trim(),
                BankAccountType = BankAccountType,
                BankAccountNumber = string.IsNullOrWhiteSpace(BankAccountNumber) ? null : BankAccountNumber.Trim(),
                BankIBAN = string.IsNullOrWhiteSpace(BankIBAN) ? null : BankIBAN.Trim(),
                Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim()
            };
            
            if (_originalEmployee != null)
            {
                employee.Id = _originalEmployee.Id;
                employee.CreatedAt = _originalEmployee.CreatedAt;
                var (success, error, _) = await _employeeService.UpdateAsync(employee);
                if (success)
                {
                    _onClose();
                }
                else
                {
                    ErrorMessage = error ?? "Error al actualizar";
                }
            }
            else
            {
                var (success, error, _) = await _employeeService.CreateAsync(employee);
                if (success)
                {
                    _onClose();
                }
                else
                {
                    ErrorMessage = error ?? "Error al crear";
                }
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
    private void Cancel()
    {
        _onClose();
    }
}