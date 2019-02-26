using System;
using System.Globalization;
using System.Windows.Data;

namespace Dionach.ShareAudit.Modules.UserInterface.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public sealed class BoolInversionConverter : IValueConverter
    {
        public static BoolInversionConverter Default { get; } = new BoolInversionConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}
