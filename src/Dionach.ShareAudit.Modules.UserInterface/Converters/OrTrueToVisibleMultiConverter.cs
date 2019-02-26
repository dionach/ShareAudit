using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Dionach.ShareAudit.Modules.UserInterface.Converters
{
    public sealed class OrTrueToVisibleMultiConverter : IMultiValueConverter
    {
        public static OrTrueToVisibleMultiConverter Default { get; } = new OrTrueToVisibleMultiConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var visible = false;
            foreach (object value in values)
            {
                if (value is bool)
                {
                    visible = visible || (bool)value;
                }
            }

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(OrTrueToVisibleMultiConverter)} can only be used in OneWay bindings");
        }
    }
}
