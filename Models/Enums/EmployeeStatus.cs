namespace Planilla_RRHH.Models.Enums;

public enum EmployeeStatus
{
    Activo = 1,
    Inactivo = 2
}

public static class EmployeeStatusExtensions
{
    public static string ToDisplayString(this EmployeeStatus status) => status switch
    {
        EmployeeStatus.Activo => "Activo",
        EmployeeStatus.Inactivo => "Inactivo",
        _ => status.ToString()
    };

    public static IEnumerable<EmployeeStatus> GetAll() => Enum.GetValues<EmployeeStatus>();
}