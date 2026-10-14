using Planilla_RRHH.Models;
using Planilla_RRHH.Security;

namespace Planilla_RRHH.Services.Interfaces;

public interface IAuthService
{
    Task<(bool success, string? error, User? user)> LoginAsync(string username, string password);
    Task<(bool success, string? error)> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<(bool success, string? error)> RequestPasswordResetAsync(string username);
    Task<(bool success, string? error, User? user)> ResetPasswordAsync(string token, string newPassword);
    Task<bool> ValidateSessionAsync();
    void Logout();
    SessionContext Session { get; }
}