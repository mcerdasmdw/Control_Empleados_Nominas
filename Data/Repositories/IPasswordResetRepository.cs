using Planilla_RRHH.Models;

namespace Planilla_RRHH.Data.Repositories;

public interface IPasswordResetRepository
{
    Task<PasswordResetToken?> GetByTokenHashAsync(byte[] tokenHash);
    Task<PasswordResetToken?> GetByUserIdAsync(int userId);
    Task<int> CreateAsync(PasswordResetToken token);
    Task<bool> MarkAsUsedAsync(int id);
    Task<bool> DeleteExpiredAsync();
    Task<bool> DeleteByUserIdAsync(int userId);
}

public class PasswordResetToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public byte[] TokenHash { get; set; } = Array.Empty<byte>();
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}