using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Planilla_RRHH.Converters;

public class StringIsNullOrEmptyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return string.IsNullOrEmpty(value?.ToString());
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public class StringIsNotNullOrEmptyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value?.ToString());
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public static class StringConverters
{
    public static readonly StringIsNullOrEmptyConverter IsNullOrEmpty = new();
    public static readonly StringIsNotNullOrEmptyConverter IsNotNullOrEmpty = new();
}