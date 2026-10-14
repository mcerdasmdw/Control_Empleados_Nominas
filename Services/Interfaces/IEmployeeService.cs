using Planilla_RRHH.Models;
using Planilla_RRHH.Models.Enums;

namespace Planilla_RRHH.Services.Interfaces;

public interface IEmployeeService
{
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByCedulaAsync(string cedula);
    Task<IReadOnlyList<Employee>> GetAllAsync(bool includeInactive = false);
    Task<IReadOnlyList<Employee>> SearchAsync(string? searchTerm, bool includeInactive = false, int page = 1, int pageSize = 50);
    Task<int> GetCountAsync(bool includeInactive = false);
    Task<(bool success, string? error, Employee? employee)> CreateAsync(Employee employee);
    Task<(bool success, string? error, Employee? employee)> UpdateAsync(Employee employee);
    Task<(bool success, string? error)> DeleteAsync(int id);
    Task<bool> ExistsByCedulaAsync(string cedula, int? excludeId = null);
    Task<IReadOnlyList<Employee>> GetRecentAsync(int count = 10);
    Task<ValidationResult> ValidateEmployeeAsync(Employee employee, int? excludeId = null);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    
    public static ValidationResult Valid() => new() { IsValid = true };
    public static ValidationResult Invalid(params string[] errors) => new() { IsValid = false, Errors = errors.ToList() };
    public static ValidationResult Invalid(IEnumerable<string> errors) => new() { IsValid = false, Errors = errors.ToList() };
}