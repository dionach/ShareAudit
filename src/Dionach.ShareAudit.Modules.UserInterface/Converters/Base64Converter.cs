using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Dionach.ShareAudit.Modules.UserInterface.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public sealed class Base64Converter : IValueConverter
    {
        public static Base64Converter Default { get; } = new Base64Converter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return Encoding.Default.GetString(System.Convert.FromBase64String(value as string));
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return System.Convert.ToBase64String(Encoding.Default.GetBytes(value as string));
            }

            return string.Empty;
        }
    }
}
