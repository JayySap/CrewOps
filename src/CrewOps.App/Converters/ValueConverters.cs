using System.Globalization;

namespace CrewOps.App.Converters;

/// <summary>
/// Converts a string to a boolean (true if not null/empty).
/// Teacher's Note: Value converters transform data between the ViewModel and View.
/// They implement IValueConverter which has two methods:
/// - Convert: ViewModel -> View (e.g., string to bool for IsVisible)
/// - ConvertBack: View -> ViewModel (often not needed, throws exception)
///
/// Example usage in XAML:
/// IsVisible="{Binding ErrorMessage, Converter={StaticResource StringToBoolConverter}}"
/// </summary>
public class StringToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Inverts a boolean value (true becomes false, false becomes true).
/// Teacher's Note: This is commonly used for:
/// - IsEnabled="{Binding IsBusy, Converter={StaticResource InvertedBoolConverter}}"
///   (disable button while loading)
/// - IsVisible when you want to show something when a condition is FALSE
/// </summary>
public class InvertedBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return false;
    }
}
