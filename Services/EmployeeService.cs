using Microsoft.Extensions.Logging;
using Planilla_RRHH.Data.Repositories;
using Planilla_RRHH.Models;
using Planilla_RRHH.Models.Enums;
using Planilla_RRHH.Security;
using Planilla_RRHH.Services.Interfaces;

namespace Planilla_RRHH.Services;

public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly ILogger<EmployeeService>? _logger;
    
    public EmployeeService(IEmployeeRepository repository, ILogger<EmployeeService>? logger = null)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public Task<Employee?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
    public Task<Employee?> GetByCedulaAsync(string cedula) => _repository.GetByCedulaAsync(cedula);
    public Task<IReadOnlyList<Employee>> GetAllAsync(bool includeInactive = false) => _repository.GetAllAsync(includeInactive);
    public Task<IReadOnlyList<Employee>> SearchAsync(string? searchTerm, bool includeInactive = false, int page = 1, int pageSize = 50) => _repository.SearchAsync(searchTerm, includeInactive, page, pageSize);
    public Task<int> GetCountAsync(bool includeInactive = false) => _repository.GetCountAsync(includeInactive);
    public Task<IReadOnlyList<Employee>> GetRecentAsync(int count = 10) => _repository.GetRecentAsync(count);
    public Task<bool> ExistsByCedulaAsync(string cedula, int? excludeId = null) => _repository.ExistsByCedulaAsync(cedula, excludeId);
    
    public async Task<(bool success, string? error, Employee? employee)> CreateAsync(Employee employee)
    {
        var validation = await ValidateEmployeeAsync(employee);
        if (!validation.IsValid)
            return (false, string.Join("; ", validation.Errors), null);
        
        employee.CreatedAt = DateTime.UtcNow;
        employee.UpdatedAt = DateTime.UtcNow;
        
        var id = await _repository.CreateAsync(employee);
        employee.Id = id;
        
        _logger?.LogInformation("Employee created: {Id} - {FullName}", id, employee.FullName);
        return (true, null, employee);
    }
    
    public async Task<(bool success, string? error, Employee? employee)> UpdateAsync(Employee employee)
    {
        var validation = await ValidateEmployeeAsync(employee, employee.Id);
        if (!validation.IsValid)
            return (false, string.Join("; ", validation.Errors), null);
        
        var existing = await _repository.GetByIdAsync(employee.Id);
        if (existing == null)
            return (false, "Empleado no encontrado", null);
        
        employee.CreatedAt = existing.CreatedAt;
        employee.UpdatedAt = DateTime.UtcNow;
        
        var updated = await _repository.UpdateAsync(employee);
        if (!updated)
            return (false, "Error al actualizar el empleado", null);
        
        _logger?.LogInformation("Employee updated: {Id} - {FullName}", employee.Id, employee.FullName);
        return (true, null, employee);
    }
    
    public async Task<(bool success, string? error)> DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return (false, "Empleado no encontrado");
        
        // Soft delete - just mark as inactive
        existing.Status = EmployeeStatus.Inactivo;
        existing.UpdatedAt = DateTime.UtcNow;
        
        var updated = await _repository.UpdateAsync(existing);
        if (!updated)
            return (false, "Error al desactivar el empleado");
        
        _logger?.LogInformation("Employee deactivated: {Id} - {FullName}", id, existing.FullName);
        return (true, null);
    }
    
    public async Task<ValidationResult> ValidateEmployeeAsync(Employee employee, int? excludeId = null)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(employee.FullName))
            errors.Add("El nombre completo es obligatorio");
        else if (employee.FullName.Length > 200)
            errors.Add("El nombre completo no puede exceder 200 caracteres");
        
        if (string.IsNullOrWhiteSpace(employee.Cedula))
            errors.Add("La cédula es obligatoria");
        else if (!CedulaValidator.Validate(employee.Cedula))
            errors.Add("La cédula no tiene un formato válido");
        else if (await _repository.ExistsByCedulaAsync(employee.Cedula, excludeId))
            errors.Add("Ya existe un empleado con esa cédula");
        
        if (!string.IsNullOrWhiteSpace(employee.Email))
        {
            if (employee.Email.Length > 255)
                errors.Add("El correo no puede exceder 255 caracteres");
            else if (!IsValidEmail(employee.Email))
                errors.Add("El formato del correo no es válido");
        }
        
        if (!string.IsNullOrWhiteSpace(employee.Phone) && employee.Phone.Length > 30)
            errors.Add("El teléfono no puede exceder 30 caracteres");
        
        if (!string.IsNullOrWhiteSpace(employee.Address) && employee.Address.Length > 500)
            errors.Add("La dirección no puede exceder 500 caracteres");
        
        if (!string.IsNullOrWhiteSpace(employee.Position) && employee.Position.Length > 100)
            errors.Add("El puesto no puede exceder 100 caracteres");
        
        if (employee.BaseSalary.HasValue && employee.BaseSalary < 0)
            errors.Add("El salario base no puede ser negativo");
        
        if (employee.HireDate.HasValue && employee.HireDate > DateTime.Today)
            errors.Add("La fecha de contratación no puede ser futura");
        
        if (!string.IsNullOrWhiteSpace(employee.BankName) && employee.BankName.Length > 100)
            errors.Add("El nombre del banco no puede exceder 100 caracteres");
        
        if (!string.IsNullOrWhiteSpace(employee.BankAccountNumber) && employee.BankAccountNumber.Length > 50)
            errors.Add("El número de cuenta no puede exceder 50 caracteres");
        
        if (!string.IsNullOrWhiteSpace(employee.BankIBAN) && employee.BankIBAN.Length > 50)
            errors.Add("El IBAN no puede exceder 50 caracteres");
        
        if (!string.IsNullOrWhiteSpace(employee.Notes) && employee.Notes.Length > 2000)
            errors.Add("Las observaciones no pueden exceder 2000 caracteres");
        
        return errors.Count > 0 ? ValidationResult.Invalid(errors) : ValidationResult.Valid();
    }
    
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}