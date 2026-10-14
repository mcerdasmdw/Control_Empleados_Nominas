using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Planilla_RRHH.Converters;

public class BoolToStringConverter : IValueConverter
{
    public string TrueValue { get; set; } = "Sí";
    public string FalseValue { get; set; } = "No";
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? TrueValue : FalseValue;
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() == TrueValue;
    }
}

public class InverseBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not true;
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not true;
    }
}

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true;
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true;
    }
}

public class BoolToColorConverter : IValueConverter
{
    public string TrueColor { get; set; } = "Green";
    public string FalseColor { get; set; } = "Gray";
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? TrueColor : FalseColor;
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public class BoolToObjectConverter<T> : IValueConverter
{
    public T TrueValue { get; set; } = default!;
    public T FalseValue { get; set; } = default!;
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? TrueValue : FalseValue;
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

// Static instances for XAML
public static class BoolConverters
{
    public static readonly BoolToStringConverter ActiveToText = new() { TrueValue = "Activo", FalseValue = "Inactivo" };
    public static readonly BoolToColorConverter ActiveToColor = new() { TrueColor = "Green", FalseColor = "Red" };
    public static readonly BoolToStringConverter ToggleButtonText = new() { TrueValue = "⏸️", FalseValue = "▶️" };
    public static readonly BoolToStringConverter ToggleTooltip = new() { TrueValue = "Desactivar", FalseValue = "Activar" };
    public static readonly BoolToStringConverter EditingSubtitle = new() { TrueValue = "Modifique los datos del empleado", FalseValue = "Complete la información del nuevo empleado" };
    public static readonly BoolToStringConverter SaveButtonText = new() { TrueValue = "Actualizar", FalseValue = "Guardar" };
    public static readonly BoolToObjectConverter<string> NavButtonClass = new() { TrueValue = "nav-enabled", FalseValue = "nav-disabled" };
}