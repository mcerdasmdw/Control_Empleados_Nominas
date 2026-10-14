namespace Planilla_RRHH.Security;

public static class CedulaValidator
{
    public static bool Validate(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula))
            return false;
        
        var cleaned = cedula.Replace("-", "").Replace(" ", "");
        
        if (!long.TryParse(cleaned, out _))
            return false;
        
        if (cleaned.Length < 9 || cleaned.Length > 12)
            return false;
        
        return ValidateCheckDigit(cleaned);
    }
    
    private static bool ValidateCheckDigit(string cedula)
    {
        int sum = 0;
        int length = cedula.Length;
        
        for (int i = 0; i < length - 1; i++)
        {
            int digit = int.Parse(cedula[i].ToString());
            int weight = (length - i) % 2 == 0 ? 2 : 1;
            int product = digit * weight;
            
            if (product >= 10)
                product = product - 9;
            
            sum += product;
        }
        
        int checkDigit = int.Parse(cedula[length - 1].ToString());
        int calculatedCheckDigit = (10 - (sum % 10)) % 10;
        
        return checkDigit == calculatedCheckDigit;
    }
    
    public static string Format(string cedula)
    {
        var cleaned = cedula.Replace("-", "").Replace(" ", "");
        if (cleaned.Length >= 9)
        {
            return $"{cleaned[..1]}-{cleaned.Substring(1, 4)}-{cleaned.Substring(5, cleaned.Length - 6)}-{cleaned[^1]}";
        }
        return cedula;
    }
}