using System;
using System.Windows.Data;

namespace Dionach.ShareAudit.Modules.UserInterface.Converters
{
    [ValueConversion(typeof(object), typeof(bool?))]
    public sealed class ComparisonConverter : IValueConverter
    {
        public static ComparisonConverter Default { get; } = new ComparisonConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.Equals(true) == true ? parameter : Binding.DoNothing;
        }
    }
}
