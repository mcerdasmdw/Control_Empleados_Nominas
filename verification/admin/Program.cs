using Microsoft.Data.Sqlite;
using Planilla_RRHH.Data;
using Planilla_RRHH.Data.Repositories;
using Planilla_RRHH.Models.Enums;
using Planilla_RRHH.Security;
using Planilla_RRHH.Services;

// R-A01 / T-A01: one-time provisioning, never invoked by application startup.
try
{
    if (args.Length == 1 && args[0] == "--self-test")
    {
        await SelfTest();
        return 0;
    }
    if (args.Length != 2 || (args[0] != "--create" && args[0] != "--verify"))
        throw new InvalidOperationException("Usage: --create|--verify <existing database path>, or --self-test.");
    var path = Path.GetFullPath(args[1]);
    if (!File.Exists(path)) throw new InvalidOperationException("Database must already exist; no schema initialization is performed.");
    var password = Environment.GetEnvironmentVariable("PLANILLA_INITIAL_ADMIN_PASSWORD");
    if (string.IsNullOrWhiteSpace(password)) throw new InvalidOperationException("PLANILLA_INITIAL_ADMIN_PASSWORD is required.");
    if (args[0] == "--create") Console.WriteLine(Create(path, password) ? "Administrator created atomically." : "Administrator already exists; unchanged.");
    await Verify(path, password);
    Console.WriteLine("PASS: real authentication, active Administrator role and full permissions.");
    return 0;
}
catch (Exception ex)
{
    // Do not emit passwords, hashes, salts, or raw SQL exception details.
    Console.Error.WriteLine(ex is SqliteException ? "FAIL: database operation failed; no partial account insertion committed." : $"FAIL: {ex.Message}");
    return 1;
}

static bool Create(string path, string password)
{
    if (password.Length < 8 || !password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
        throw new InvalidOperationException("Initial password must satisfy the existing password requirements.");
    using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = path, Mode = SqliteOpenMode.ReadWrite, ForeignKeys = true }.ToString());
    connection.Open();
    using var transaction = connection.BeginTransaction();
    using var command = connection.CreateCommand();
    command.Transaction = transaction;
    command.CommandText = "SELECT COUNT(*) FROM Roles WHERE Id = 1 AND Name = 'Administrador' AND Permissions = @permissions;";
    command.Parameters.AddWithValue("@permissions", (int)PermissionFlags.AdminAll);
    if (Convert.ToInt32(command.ExecuteScalar()) != 1) throw new InvalidOperationException("Expected Administrator role and permissions are absent; database unchanged.");
    command.Parameters.Clear();
    command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @username COLLATE NOCASE;";
    command.Parameters.AddWithValue("@username", "Administrador");
    if (Convert.ToInt32(command.ExecuteScalar()) != 0)
    {
        transaction.Commit();
        return false;
    }
    var (hash, salt, parameters) = Argon2Hasher.HashPassword(password);
    command.CommandText = "INSERT INTO Users (Username, PasswordHash, PasswordSalt, Argon2Params, IsActive, CreatedAt, UpdatedAt) VALUES (@username, @hash, @salt, @params, 1, @now, @now); SELECT last_insert_rowid();";
    command.Parameters.AddWithValue("@hash", hash);
    command.Parameters.AddWithValue("@salt", salt);
    command.Parameters.AddWithValue("@params", parameters);
    command.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("O"));
    var id = Convert.ToInt64(command.ExecuteScalar());
    command.Parameters.Clear();
    command.CommandText = "INSERT INTO UserRoles (UserId, RoleId) VALUES (@id, 1);";
    command.Parameters.AddWithValue("@id", id);
    command.ExecuteNonQuery();
    transaction.Commit();
    return true;
}

static async Task Verify(string path, string password)
{
    using var factory = new SqliteConnectionFactory(path);
    var database = new Database(factory);
    var session = new SessionContext();
    var auth = new AuthService(new UserRepository(database), new PasswordResetRepository(database), session);
    var result = await auth.LoginAsync("Administrador", password);
    if (!result.success || result.user?.IsActive != true || !session.IsAuthenticated || session.CurrentRole?.Id != 1 || session.CurrentRole.Name != "Administrador" || session.Permissions != PermissionFlags.AdminAll)
        throw new InvalidOperationException("Administrator authentication or full permissions verification failed; existing credentials were not reset.");
}

static async Task SelfTest()
{
    var directory = Path.Combine(Path.GetTempPath(), "PlanillaAdminTest-" + Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(directory);
    var path = Path.Combine(directory, "synthetic.db");
    const string password = "SyntheticTest123";
    try
    {
        using (var factory = new SqliteConnectionFactory(path)) new Database(factory).Initialize();
        if (!Create(path, password)) throw new InvalidOperationException("Synthetic first insertion failed.");
        using var factory2 = new SqliteConnectionFactory(path);
        var repository = new UserRepository(new Database(factory2));
        var before = (await repository.GetByUsernameAsync("Administrador"))!;
        if (Create(path, "DifferentTest123")) throw new InvalidOperationException("Synthetic repeat unexpectedly inserted.");
        var after = (await repository.GetByUsernameAsync("Administrador"))!;
        var role = await repository.GetUserRoleAsync(after.Id);
        if (before.Id != after.Id || !before.PasswordHash.SequenceEqual(after.PasswordHash) || !before.PasswordSalt.SequenceEqual(after.PasswordSalt) || before.Argon2Params != after.Argon2Params || role?.Id != 1 || (await repository.GetAllAsync()).Count != 1)
            throw new InvalidOperationException("Synthetic idempotence failed.");
        await Verify(path, password);
        var rejected = new AuthService(repository, new PasswordResetRepository(new Database(factory2)), new SessionContext());
        if ((await rejected.LoginAsync("Administrador", "DifferentTest123")).success) throw new InvalidOperationException("Synthetic replacement password accepted.");
        Console.WriteLine("PASS: synthetic atomic insertion, unchanged repeat/hash/salt/role, real authentication/full permissions, replacement password rejected.");
    }
    finally
    {
        SqliteConnection.ClearAllPools();
        Directory.Delete(directory, recursive: true);
    }
}
