using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Planilla_RRHH.Converters;

public class BoolToDoubleConverter : IValueConverter
{
    public double TrueValue { get; set; } = 260;
    public double FalseValue { get; set; } = 60;
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? TrueValue : FalseValue;
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public class DoubleConverters
{
    public static readonly BoolToDoubleConverter SidebarWidth = new() { TrueValue = 260, FalseValue = 60 };
}