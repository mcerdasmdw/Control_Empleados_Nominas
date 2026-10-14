using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Planilla_RRHH.Converters;

public class NullToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var param = parameter?.ToString();
        return param switch
        {
            "IsNotNull" => value != null,
            _ => value == null
        };
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public static class NullConverters
{
    public static readonly NullToBoolConverter IsNotNull = new();
}