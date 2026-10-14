using Avalonia.Data.Converters;
using Planilla_RRHH.Models.Enums;
using System;
using System.Globalization;

namespace Planilla_RRHH.Converters;

public class EnumToDisplayStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            SalaryType st => st.ToDisplayString(),
            EmployeeStatus es => es.ToDisplayString(),
            BankAccountType bat => bat.ToDisplayString(),
            _ => value?.ToString() ?? string.Empty
        };
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public static class EnumConverters
{
    public static readonly EnumToDisplayStringConverter SalaryTypeToString = new();
    public static readonly EnumToDisplayStringConverter EmployeeStatusToString = new();
    public static readonly EnumToDisplayStringConverter BankAccountTypeToString = new();
}