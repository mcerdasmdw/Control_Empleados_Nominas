using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Planilla_RRHH.Models;
using Planilla_RRHH.Models.Enums;
using Planilla_RRHH.Data;

namespace Planilla_RRHH.Data.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly Database _database;
    private readonly ILogger<EmployeeRepository>? _logger;
    
    public EmployeeRepository(Database database, ILogger<EmployeeRepository>? logger = null)
    {
        _database = database;
        _logger = logger;
    }
    
    public async Task<Employee?> GetByIdAsync(int id)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = GetSelectSql() + " WHERE Id = @id;";
        command.Parameters.AddWithValue("@id", id);
        
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadEmployee(reader);
        }
        return null;
    }
    
    public async Task<Employee?> GetByCedulaAsync(string cedula)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = GetSelectSql() + " WHERE Cedula = @cedula;";
        command.Parameters.AddWithValue("@cedula", cedula);
        
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadEmployee(reader);
        }
        return null;
    }
    
    public async Task<IReadOnlyList<Employee>> GetAllAsync(bool includeInactive = false)
    {
        var employees = new List<Employee>();
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        
        var sql = GetSelectSql();
        if (!includeInactive)
        {
            sql += " WHERE Status = @status";
        }
        sql += " ORDER BY FullName;";
        
        command.CommandText = sql;
        if (!includeInactive)
        {
            command.Parameters.AddWithValue("@status", (int)EmployeeStatus.Activo);
        }
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            employees.Add(ReadEmployee(reader));
        }
        return employees;
    }
    
    public async Task<IReadOnlyList<Employee>> SearchAsync(string? searchTerm, bool includeInactive = false, int page = 1, int pageSize = 50)
    {
        var employees = new List<Employee>();
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        
        var sql = GetSelectSql();
        var conditions = new List<string>();
        var paramIndex = 0;
        
        if (!includeInactive)
        {
            conditions.Add("Status = @status");
            command.Parameters.AddWithValue("@status", (int)EmployeeStatus.Activo);
        }
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            conditions.Add($"(FullName LIKE @search{paramIndex} OR Cedula LIKE @search{paramIndex})");
            command.Parameters.AddWithValue($"@search{paramIndex}", $"%{searchTerm}%");
            paramIndex++;
        }
        
        if (conditions.Count > 0)
        {
            sql += " WHERE " + string.Join(" AND ", conditions);
        }
        
        sql += " ORDER BY FullName LIMIT @limit OFFSET @offset;";
        command.CommandText = sql;
        command.Parameters.AddWithValue("@limit", pageSize);
        command.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            employees.Add(ReadEmployee(reader));
        }
        return employees;
    }
    
    public async Task<int> GetCountAsync(bool includeInactive = false)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        
        var sql = "SELECT COUNT(*) FROM Employees";
        if (!includeInactive)
        {
            sql += " WHERE Status = @status;";
            command.Parameters.AddWithValue("@status", (int)EmployeeStatus.Activo);
        }
        else
        {
            sql += ";";
        }
        command.CommandText = sql;
        
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
    
    public async Task<int> CreateAsync(Employee employee)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Employees (FullName, Cedula, Phone, Email, Address, Position, BaseSalary, SalaryType, HireDate, Status, BankName, BankAccountType, BankAccountNumber, BankIBAN, Notes, CreatedAt, UpdatedAt)
            VALUES (@fullName, @cedula, @phone, @email, @address, @position, @baseSalary, @salaryType, @hireDate, @status, @bankName, @bankAccountType, @bankAccountNumber, @bankIBAN, @notes, @created, @updated);
            SELECT last_insert_rowid();";
        
        AddEmployeeParameters(command, employee);
        
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
    
    public async Task<bool> UpdateAsync(Employee employee)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Employees SET 
                FullName = @fullName,
                Cedula = @cedula,
                Phone = @phone,
                Email = @email,
                Address = @address,
                Position = @position,
                BaseSalary = @baseSalary,
                SalaryType = @salaryType,
                HireDate = @hireDate,
                Status = @status,
                BankName = @bankName,
                BankAccountType = @bankAccountType,
                BankAccountNumber = @bankAccountNumber,
                BankIBAN = @bankIBAN,
                Notes = @notes,
                UpdatedAt = @updated
            WHERE Id = @id;";
        
        command.Parameters.AddWithValue("@id", employee.Id);
        AddEmployeeParameters(command, employee, includeId: false);
        command.Parameters.AddWithValue("@updated", DateTime.UtcNow.ToString("O"));
        
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Employees WHERE Id = @id;";
        command.Parameters.AddWithValue("@id", id);
        
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }
    
    public async Task<bool> ExistsByCedulaAsync(string cedula, int? excludeId = null)
    {
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        
        if (excludeId.HasValue)
        {
            command.CommandText = "SELECT COUNT(1) FROM Employees WHERE Cedula = @cedula AND Id != @excludeId;";
            command.Parameters.AddWithValue("@excludeId", excludeId.Value);
        }
        else
        {
            command.CommandText = "SELECT COUNT(1) FROM Employees WHERE Cedula = @cedula;";
        }
        command.Parameters.AddWithValue("@cedula", cedula);
        
        var count = await command.ExecuteScalarAsync();
        return Convert.ToInt64(count) > 0;
    }
    
    public async Task<IReadOnlyList<Employee>> GetRecentAsync(int count = 10)
    {
        var employees = new List<Employee>();
        using var connection = _database.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = GetSelectSql() + " ORDER BY CreatedAt DESC LIMIT @count;";
        command.Parameters.AddWithValue("@count", count);
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            employees.Add(ReadEmployee(reader));
        }
        return employees;
    }
    
    private static string GetSelectSql()
    {
        return @"SELECT Id, FullName, Cedula, Phone, Email, Address, Position, BaseSalary, SalaryType, HireDate, Status, BankName, BankAccountType, BankAccountNumber, BankIBAN, Notes, CreatedAt, UpdatedAt FROM Employees";
    }
    
    private static void AddEmployeeParameters(SqliteCommand command, Employee employee, bool includeId = true)
    {
        if (includeId)
            command.Parameters.AddWithValue("@id", employee.Id);
        command.Parameters.AddWithValue("@fullName", employee.FullName);
        command.Parameters.AddWithValue("@cedula", employee.Cedula);
        command.Parameters.AddWithValue("@phone", employee.Phone ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@email", employee.Email ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@address", employee.Address ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@position", employee.Position ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@baseSalary", employee.BaseSalary.HasValue ? (object)employee.BaseSalary.Value : DBNull.Value);
        command.Parameters.AddWithValue("@salaryType", employee.SalaryType.HasValue ? (int)employee.SalaryType.Value : (object)DBNull.Value);
        command.Parameters.AddWithValue("@hireDate", employee.HireDate.HasValue ? employee.HireDate.Value.ToString("O") : (object)DBNull.Value);
        command.Parameters.AddWithValue("@status", (int)employee.Status);
        command.Parameters.AddWithValue("@bankName", employee.BankName ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@bankAccountType", employee.BankAccountType.HasValue ? (int)employee.BankAccountType.Value : (object)DBNull.Value);
        command.Parameters.AddWithValue("@bankAccountNumber", employee.BankAccountNumber ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@bankIBAN", employee.BankIBAN ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@notes", employee.Notes ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@created", employee.CreatedAt.ToString("O"));
        command.Parameters.AddWithValue("@updated", employee.UpdatedAt.ToString("O"));
    }
    
    private static Employee ReadEmployee(SqliteDataReader reader)
    {
        return new Employee
        {
            Id = reader.GetInt32(0),
            FullName = reader.GetString(1),
            Cedula = reader.GetString(2),
            Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
            Email = reader.IsDBNull(4) ? null : reader.GetString(4),
            Address = reader.IsDBNull(5) ? null : reader.GetString(5),
            Position = reader.IsDBNull(6) ? null : reader.GetString(6),
            BaseSalary = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
            SalaryType = reader.IsDBNull(8) ? null : (SalaryType?)reader.GetInt32(8),
            HireDate = reader.IsDBNull(9) ? null : DateTime.Parse(reader.GetString(9)),
            Status = (EmployeeStatus)reader.GetInt32(10),
            BankName = reader.IsDBNull(11) ? null : reader.GetString(11),
            BankAccountType = reader.IsDBNull(12) ? null : (BankAccountType?)reader.GetInt32(12),
            BankAccountNumber = reader.IsDBNull(13) ? null : reader.GetString(13),
            BankIBAN = reader.IsDBNull(14) ? null : reader.GetString(14),
            Notes = reader.IsDBNull(15) ? null : reader.GetString(15),
            CreatedAt = DateTime.Parse(reader.GetString(16)),
            UpdatedAt = DateTime.Parse(reader.GetString(17))
        };
    }
}