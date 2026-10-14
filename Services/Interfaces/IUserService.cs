using Planilla_RRHH.Models;
using Planilla_RRHH.Models.Enums;

namespace Planilla_RRHH.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<(bool success, string? error, User? user)> CreateAsync(User user, string password, int roleId);
    Task<(bool success, string? error)> UpdateAsync(User user);
    Task<(bool success, string? error)> DeleteAsync(int id);
    Task<(bool success, string? error)> ChangePasswordAsync(int userId, string newPassword);
    Task<(bool success, string? error)> SetRoleAsync(int userId, int roleId);
    Task<Role?> GetRoleAsync(int userId);
    Task<IReadOnlyList<Role>> GetAllRolesAsync();
    Task<bool> ExistsByUsernameAsync(string username, int? excludeId = null);
    Task<bool> ExistsByEmailAsync(string email, int? excludeId = null);
    Task<bool> IsLastAdminAsync(int userId);
}