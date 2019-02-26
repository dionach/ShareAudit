using System;
using System.Windows;
using System.Windows.Data;

namespace Dionach.ShareAudit.Modules.UserInterface.Converters
{
    public sealed class AndTrueToVisibleMultiConverter : IMultiValueConverter
    {
        public static AndTrueToVisibleMultiConverter Default { get; } = new AndTrueToVisibleMultiConverter();

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visible = true;
            foreach (object value in values)
            {
                if (value is bool)
                {
                    visible = visible && (bool)value;
                }
            }

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(AndTrueToVisibleMultiConverter)} can only be used in OneWay bindings");
        }
    }
}
