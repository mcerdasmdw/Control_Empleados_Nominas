using System.Security.Cryptography;

namespace Planilla_RRHH.Security;

public static class TokenGenerator
{
    private const int TokenByteSize = 32;
    
    public static string GenerateSecureToken()
    {
        var bytes = new byte[TokenByteSize];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
    
    public static string GenerateNumericCode(int length = 6)
    {
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = (char)('0' + (bytes[i] % 10));
        }
        return new string(result);
    }
    
    public static byte[] HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
    }
    
    public static bool VerifyToken(string token, byte[] tokenHash)
    {
        var computedHash = HashToken(token);
        return CryptographicOperations.FixedTimeEquals(computedHash, tokenHash);
    }
}