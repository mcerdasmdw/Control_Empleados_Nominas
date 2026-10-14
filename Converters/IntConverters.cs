using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Planilla_RRHH.Converters;

public class IntToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            var param = parameter?.ToString();
            return param switch
            {
                "IsZero" => intValue == 0,
                "IsGreaterThanZero" => intValue > 0,
                "IsGreaterThanOne" => intValue > 1,
                _ => intValue != 0
            };
        }
        return false;
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public static class IntConverters
{
    public static readonly IntToBoolConverter IsZero = new();
    public static readonly IntToBoolConverter IsGreaterThanZero = new();
    public static readonly IntToBoolConverter IsGreaterThanOne = new();
}