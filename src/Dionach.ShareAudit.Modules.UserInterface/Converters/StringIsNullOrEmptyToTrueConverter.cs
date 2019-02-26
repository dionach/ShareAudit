using System;
using System.Windows.Data;

namespace Dionach.ShareAudit.Modules.UserInterface.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public sealed class StringIsNullOrEmptyToTrueConverter : IValueConverter
    {
        public static StringIsNullOrEmptyToTrueConverter Default { get; } = new StringIsNullOrEmptyToTrueConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(StringIsNullOrEmptyToTrueConverter)} can only be used in OneWay bindings");
        }
    }
}
