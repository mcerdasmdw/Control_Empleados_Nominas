using Microsoft.Extensions.Logging;
using Planilla_RRHH.Data.Repositories;
using Planilla_RRHH.Models;
using Planilla_RRHH.Security;
using Planilla_RRHH.Services.Interfaces;

namespace Planilla_RRHH.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetRepository _resetRepository;
    private readonly SessionContext _session;
    private readonly ILogger<AuthService>? _logger;
    
    public SessionContext Session => _session;
    
    public AuthService(
        IUserRepository userRepository,
        IPasswordResetRepository resetRepository,
        SessionContext session,
        ILogger<AuthService>? logger = null)
    {
        _userRepository = userRepository;
        _resetRepository = resetRepository;
        _session = session;
        _logger = logger;
    }
    
    public async Task<(bool success, string? error, User? user)> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return (false, "Usuario y contraseña son obligatorios", null);
        }
        
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            _logger?.LogWarning("Login failed: user not found {Username}", username);
            return (false, "Credenciales inválidas", null);
        }
        
        if (!user.IsActive)
        {
            _logger?.LogWarning("Login failed: user inactive {Username}", username);
            return (false, "La cuenta está desactivada", null);
        }
        
        var isValid = Argon2Hasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt, user.Argon2Params);
        if (!isValid)
        {
            _logger?.LogWarning("Login failed: invalid password {Username}", username);
            return (false, "Credenciales inválidas", null);
        }
        
        var role = await _userRepository.GetUserRoleAsync(user.Id);
        if (role == null)
        {
            _logger?.LogError("User {UserId} has no assigned role", user.Id);
            return (false, "El usuario no tiene rol asignado", null);
        }
        
        _session.SetUser(user, role);
        _logger?.LogInformation("User {Username} logged in successfully", username);
        
        return (true, null, user);
    }
    
    public async Task<(bool success, string? error)> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return (false, "Usuario no encontrado");
        
        var isValid = Argon2Hasher.VerifyPassword(currentPassword, user.PasswordHash, user.PasswordSalt, user.Argon2Params);
        if (!isValid)
            return (false, "Contraseña actual incorrecta");
        
        var validationError = ValidatePassword(newPassword);
        if (validationError != null)
            return (false, validationError);
        
        var (hash, salt, paramsJson) = Argon2Hasher.HashPassword(newPassword);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;
        user.Argon2Params = paramsJson;
        user.UpdatedAt = DateTime.UtcNow;
        
        var updated = await _userRepository.UpdateAsync(user);
        if (!updated)
            return (false, "Error al actualizar la contraseña");
        
        _logger?.LogInformation("Password changed for user {UserId}", userId);
        return (true, null);
    }
    
    public async Task<(bool success, string? error)> RequestPasswordResetAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            // Don't reveal if user exists
            return (true, null);
        }
        
        await _resetRepository.DeleteByUserIdAsync(user.Id);
        
        var token = TokenGenerator.GenerateSecureToken();
        var tokenHash = TokenGenerator.HashToken(token);
        var expiresAt = DateTime.UtcNow.AddHours(1);
        
        var resetToken = new Planilla_RRHH.Data.Repositories.PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
        
        await _resetRepository.CreateAsync(resetToken);
        
        // In production, this would be sent via email
        // For now, log to console for testing
        Console.WriteLine($"=== PASSWORD RESET TOKEN FOR {username} ===");
        Console.WriteLine($"Token: {token}");
        Console.WriteLine($"Expires: {expiresAt:O}");
        Console.WriteLine($"===========================================");
        
        _logger?.LogInformation("Password reset requested for user {Username}", username);
        return (true, null);
    }
    
    public async Task<(bool success, string? error, User? user)> ResetPasswordAsync(string token, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
            return (false, "Token y nueva contraseña son obligatorios", null);
        
        var tokenHash = TokenGenerator.HashToken(token);
        var resetToken = await _resetRepository.GetByTokenHashAsync(tokenHash);
        
        if (resetToken == null)
            return (false, "Token inválido o expirado", null);
        
        var validationError = ValidatePassword(newPassword);
        if (validationError != null)
            return (false, validationError, null);
        
        var user = await _userRepository.GetByIdAsync(resetToken.UserId);
        if (user == null)
            return (false, "Usuario no encontrado", null);
        
        var (hash, salt, paramsJson) = Argon2Hasher.HashPassword(newPassword);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;
        user.Argon2Params = paramsJson;
        user.UpdatedAt = DateTime.UtcNow;
        
        var updated = await _userRepository.UpdateAsync(user);
        if (!updated)
            return (false, "Error al actualizar la contraseña", null);
        
        await _resetRepository.MarkAsUsedAsync(resetToken.Id);
        
        _logger?.LogInformation("Password reset completed for user {UserId}", user.Id);
        return (true, null, user);
    }
    
    public Task<bool> ValidateSessionAsync()
    {
        return Task.FromResult(_session.IsAuthenticated);
    }
    
    public void Logout()
    {
        var username = _session.Username;
        _session.Clear();
        _logger?.LogInformation("User {Username} logged out", username);
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