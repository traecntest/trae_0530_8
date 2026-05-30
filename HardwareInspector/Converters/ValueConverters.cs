using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HardwareInspector.Converters;

public class TemperatureToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is float temp)
        {
            if (temp >= 80) return new SolidColorBrush(Color.FromRgb(255, 71, 87));
            if (temp >= 60) return new SolidColorBrush(Color.FromRgb(255, 176, 32));
            return new SolidColorBrush(Color.FromRgb(0, 201, 167));
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class LoadToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is float load)
        {
            if (load >= 90) return new SolidColorBrush(Color.FromRgb(255, 71, 87));
            if (load >= 70) return new SolidColorBrush(Color.FromRgb(255, 176, 32));
            return new SolidColorBrush(Color.FromRgb(0, 201, 167));
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class StatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            return status.ToLowerInvariant() switch
            {
                "ok" => new SolidColorBrush(Color.FromRgb(0, 201, 167)),
                "error" => new SolidColorBrush(Color.FromRgb(255, 71, 87)),
                "connected" => new SolidColorBrush(Color.FromRgb(0, 201, 167)),
                "disconnected" => new SolidColorBrush(Color.FromRgb(255, 176, 32)),
                _ => new SolidColorBrush(Color.FromRgb(136, 153, 170))
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class BytesToReadableConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < suffixes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {suffixes[order]}";
        }
        return "0 B";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class SensorTypeToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is HardwareInspector.Models.SensorType type)
        {
            return type switch
            {
                HardwareInspector.Models.SensorType.Temperature => "🌡️",
                HardwareInspector.Models.SensorType.Voltage => "⚡",
                HardwareInspector.Models.SensorType.FanSpeed => "🌀",
                HardwareInspector.Models.SensorType.Load => "📊",
                HardwareInspector.Models.SensorType.Power => "💡",
                HardwareInspector.Models.SensorType.Clock => "⏱️",
                _ => "📈"
            };
        }
        return "📈";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? Visibility.Visible : Visibility.Collapsed;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class CountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int count)
            return count > 0 ? Visibility.Visible : Visibility.Collapsed;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
