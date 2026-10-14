namespace Planilla_RRHH.Models.Enums;

public enum SalaryType
{
    Mensual = 1,
    Quincenal = 2,
    Semanal = 3,
    Diario = 4,
    PorHora = 5
}

public static class SalaryTypeExtensions
{
    public static string ToDisplayString(this SalaryType type) => type switch
    {
        SalaryType.Mensual => "Mensual",
        SalaryType.Quincenal => "Quincenal",
        SalaryType.Semanal => "Semanal",
        SalaryType.Diario => "Diario",
        SalaryType.PorHora => "Por hora",
        _ => type.ToString()
    };

    public static IEnumerable<SalaryType> GetAll() => Enum.GetValues<SalaryType>();
}