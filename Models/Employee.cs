using Planilla_RRHH.Models.Enums;

namespace Planilla_RRHH.Models;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Position { get; set; }
    public decimal? BaseSalary { get; set; }
    public SalaryType? SalaryType { get; set; }
    public DateTime? HireDate { get; set; }
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Activo;
    public string? BankName { get; set; }
    public BankAccountType? BankAccountType { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankIBAN { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}