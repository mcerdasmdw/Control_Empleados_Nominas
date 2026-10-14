using System.Security.Cryptography;
using System.Text.Json;
using Konscious.Security.Cryptography;

namespace Planilla_RRHH.Security;

public sealed class Argon2Parameters
{
    public int MemoryCost { get; init; } = 65536; // 64 MB
    public int TimeCost { get; init; } = 3;
    public int Parallelism { get; init; } = 4;
    public int SaltSize { get; init; } = 16;
    public int HashSize { get; init; } = 32;
}

public static class Argon2Hasher
{
    private static readonly Argon2Parameters DefaultParams = new();
    
    public static (byte[] hash, byte[] salt, string paramsJson) HashPassword(string password, Argon2Parameters? parameters = null)
    {
        var p = parameters ?? DefaultParams;
        var salt = GenerateSalt(p.SaltSize);
        
        using var argon2 = new Argon2id(System.Text.Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = p.Parallelism,
            Iterations = p.TimeCost,
            MemorySize = p.MemoryCost
        };
        
        var hash = argon2.GetBytes(p.HashSize);
        var paramsJson = JsonSerializer.Serialize(p);
        
        return (hash, salt, paramsJson);
    }
    
    public static bool VerifyPassword(string password, byte[] hash, byte[] salt, string paramsJson)
    {
        try
        {
            var p = JsonSerializer.Deserialize<Argon2Parameters>(paramsJson) ?? DefaultParams;
            
            using var argon2 = new Argon2id(System.Text.Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = p.Parallelism,
                Iterations = p.TimeCost,
                MemorySize = p.MemoryCost
            };
            
            var computedHash = argon2.GetBytes(p.HashSize);
            return CryptographicOperations.FixedTimeEquals(hash, computedHash);
        }
        catch
        {
            return false;
        }
    }
    
    public static byte[] GenerateSalt(int size = 16)
    {
        var salt = new byte[size];
        RandomNumberGenerator.Fill(salt);
        return salt;
    }
    
    public static string GenerateAdminHash(string password)
    {
        var (hash, salt, paramsJson) = HashPassword(password);
        return $"{Convert.ToBase64String(hash)}|{Convert.ToBase64String(salt)}|{paramsJson}";
    }
}