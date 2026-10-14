using Planilla_RRHH.Models;

namespace Planilla_RRHH.Data.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int id);
    Task<Role?> GetByNameAsync(string name);
    Task<IReadOnlyList<Role>> GetAllAsync();
    Task<int> CreateAsync(Role role);
    Task<bool> UpdateAsync(Role role);
    Task<bool> DeleteAsync(int id);
}