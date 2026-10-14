using Microsoft.Extensions.Logging;
using Planilla_RRHH.Data.Repositories;
using Planilla_RRHH.Models;
using Planilla_RRHH.Models.Enums;
using Planilla_RRHH.Security;
using Planilla_RRHH.Services.Interfaces;

namespace Planilla_RRHH.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<UserService>? _logger;
    
    public UserService(IUserRepository userRepository, IRoleRepository roleRepository, ILogger<UserService>? logger = null)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _logger = logger;
    }
    
    public Task<User?> GetByIdAsync(int id) => _userRepository.GetByIdAsync(id);
    public Task<User?> GetByUsernameAsync(string username) => _userRepository.GetByUsernameAsync(username);
    public Task<IReadOnlyList<User>> GetAllAsync() => _userRepository.GetAllAsync();
    public Task<Role?> GetRoleAsync(int userId) => _userRepository.GetUserRoleAsync(userId);
    public Task<IReadOnlyList<Role>> GetAllRolesAsync() => _roleRepository.GetAllAsync();
    public Task<bool> ExistsByUsernameAsync(string username, int? excludeId = null) => _userRepository.ExistsByUsernameAsync(username);
    public Task<bool> ExistsByEmailAsync(string email, int? excludeId = null) => _userRepository.ExistsByEmailAsync(email, excludeId);
    
    public async Task<(bool success, string? error, User? user)> CreateAsync(User user, string password, int roleId)
    {
        if (string.IsNullOrWhiteSpace(user.Username))
            return (false, "El nombre de usuario es obligatorio", null);
        
        if (await _userRepository.ExistsByUsernameAsync(user.Username))
            return (false, "El nombre de usuario ya existe", null);
        
        if (!string.IsNullOrWhiteSpace(user.Email) && await _userRepository.ExistsByEmailAsync(user.Email))
            return (false, "El correo electrónico ya está registrado", null);
        
        var passwordError = ValidatePassword(password);
        if (passwordError != null)
            return (false, passwordError, null);
        
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            return (false, "El rol seleccionado no existe", null);
        
        var (hash, salt, paramsJson) = Argon2Hasher.HashPassword(password);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;
        user.Argon2Params = paramsJson;
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        
        var userId = await _userRepository.CreateAsync(user);
        await _userRepository.AssignRoleAsync(userId, roleId);
        
        user.Id = userId;
        _logger?.LogInformation("User created: {Id} - {Username} with role {RoleId}", userId, user.Username, roleId);
        return (true, null, user);
    }
    
    public async Task<(bool success, string? error)> UpdateAsync(User user)
    {
        var existing = await _userRepository.GetByIdAsync(user.Id);
        if (existing == null)
            return (false, "Usuario no encontrado");
        
        if (await _userRepository.ExistsByUsernameAsync(user.Username, user.Id))
            return (false, "El nombre de usuario ya existe");
        
        if (!string.IsNullOrWhiteSpace(user.Email) && await _userRepository.ExistsByEmailAsync(user.Email, user.Id))
            return (false, "El correo electrónico ya está registrado");
        
        existing.Username = user.Username;
        existing.Email = user.Email;
        existing.IsActive = user.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;
        
        var updated = await _userRepository.UpdateAsync(existing);
        if (!updated)
            return (false, "Error al actualizar el usuario");
        
        _logger?.LogInformation("User updated: {Id} - {Username}", user.Id, user.Username);
        return (true, null);
    }
    
    public async Task<(bool success, string? error)> DeleteAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return (false, "Usuario no encontrado");
        
        if (await IsLastAdminAsync(id))
            return (false, "No se puede eliminar el último administrador activo");
        
        var deleted = await _userRepository.DeleteAsync(id);
        if (!deleted)
            return (false, "Error al eliminar el usuario");
        
        _logger?.LogInformation("User deleted: {Id} - {Username}", id, user.Username);
        return (true, null);
    }
    
    public async Task<(bool success, string? error)> ChangePasswordAsync(int userId, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return (false, "Usuario no encontrado");
        
        var passwordError = ValidatePassword(newPassword);
        if (passwordError != null)
            return (false, passwordError);
        
        var (hash, salt, paramsJson) = Argon2Hasher.HashPassword(newPassword);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;
        user.Argon2Params = paramsJson;
        user.UpdatedAt = DateTime.UtcNow;
        
        var updated = await _userRepository.UpdateAsync(user);
        if (!updated)
            return (false, "Error al cambiar la contraseña");
        
        _logger?.LogInformation("Password changed by admin for user {UserId}", userId);
        return (true, null);
    }
    
    public async Task<(bool success, string? error)> SetRoleAsync(int userId, int roleId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return (false, "Usuario no encontrado");
        
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            return (false, "El rol seleccionado no existe");
        
        // Check if removing admin role from last admin
        var currentRole = await _userRepository.GetUserRoleAsync(userId);
        if (currentRole != null && currentRole.Permissions.HasPermission(PermissionFlags.UserManageRoles))
        {
            var allUsers = await _userRepository.GetAllAsync();
            var adminCount = 0;
            foreach (var u in allUsers)
            {
                if (!u.IsActive) continue;
                var userRole = await _userRepository.GetUserRoleAsync(u.Id);
                if (userRole?.Permissions.HasPermission(PermissionFlags.UserManageRoles) == true)
                    adminCount++;
            }
            
            if (adminCount <= 1 && !role.Permissions.HasPermission(PermissionFlags.UserManageRoles))
                return (false, "No se puede quitar el rol de administrador al último administrador activo");
        }
        
        var assigned = await _userRepository.AssignRoleAsync(userId, roleId);
        if (!assigned)
            return (false, "Error al asignar el rol");
        
        _logger?.LogInformation("Role changed for user {UserId} to {RoleId}", userId, roleId);
        return (true, null);
    }
    
    public async Task<bool> IsLastAdminAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;
        
        var currentRole = await _userRepository.GetUserRoleAsync(userId);
        if (currentRole == null || !currentRole.Permissions.HasPermission(PermissionFlags.UserManageRoles))
            return false;
        
        var adminCount = 0;
        var allUsers = await _userRepository.GetAllAsync();
        foreach (var u in allUsers)
        {
            if (!u.IsActive) continue;
            var role = await _userRepository.GetUserRoleAsync(u.Id);
            if (role?.Permissions.HasPermission(PermissionFlags.UserManageRoles) == true)
                adminCount++;
        }
        
        return adminCount <= 1;
    }
    
    private static string? ValidatePassword(string password)
    {
        if (password.Length < 8)
            return "La contraseña debe tener al menos 8 caracteres";
        if (!password.Any(char.IsUpper))
            return "La contraseña debe contener al menos una mayúscula";
        if (!password.Any(char.IsLower))
            return "La contraseña debe contener al menos una minúscula";
        if (!password.Any(char.IsDigit))
            return "La contraseña debe contener al menos un número";
        return null;
    }
}