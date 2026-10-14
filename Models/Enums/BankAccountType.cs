namespace Planilla_RRHH.Models.Enums;

public enum BankAccountType
{
    Ahorros = 1,
    Corriente = 2
}

public static class BankAccountTypeExtensions
{
    public static string ToDisplayString(this BankAccountType type) => type switch
    {
        BankAccountType.Ahorros => "Ahorros",
        BankAccountType.Corriente => "Corriente",
        _ => type.ToString()
    };

    public static IEnumerable<BankAccountType> GetAll() => Enum.GetValues<BankAccountType>();
}