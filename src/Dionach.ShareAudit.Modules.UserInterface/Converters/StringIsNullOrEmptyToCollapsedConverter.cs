using System;
using System.Windows;
using System.Windows.Data;

namespace Dionach.ShareAudit.Modules.UserInterface.Converters
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public sealed class StringIsNullOrEmptyToCollapsedConverter : IValueConverter
    {
        public static StringIsNullOrEmptyToCollapsedConverter Default { get; } = new StringIsNullOrEmptyToCollapsedConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(StringIsNullOrEmptyToCollapsedConverter)} can only be used in OneWay bindings");
        }
    }
}
