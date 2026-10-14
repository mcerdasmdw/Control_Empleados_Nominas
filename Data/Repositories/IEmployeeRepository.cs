using Planilla_RRHH.Models;

namespace Planilla_RRHH.Data.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByCedulaAsync(string cedula);
    Task<IReadOnlyList<Employee>> GetAllAsync(bool includeInactive = false);
    Task<IReadOnlyList<Employee>> SearchAsync(string? searchTerm, bool includeInactive = false, int page = 1, int pageSize = 50);
    Task<int> GetCountAsync(bool includeInactive = false);
    Task<int> CreateAsync(Employee employee);
    Task<bool> UpdateAsync(Employee employee);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByCedulaAsync(string cedula, int? excludeId = null);
    Task<IReadOnlyList<Employee>> GetRecentAsync(int count = 10);
}