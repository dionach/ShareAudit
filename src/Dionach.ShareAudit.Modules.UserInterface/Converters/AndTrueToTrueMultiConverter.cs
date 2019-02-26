using System;
using System.Windows.Data;

namespace Dionach.ShareAudit.Modules.UserInterface.Converters
{
    public sealed class AndTrueToTrueMultiConverter : IMultiValueConverter
    {
        public static AndTrueToTrueMultiConverter Default { get; } = new AndTrueToTrueMultiConverter();

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var andTrue = true;
            foreach (object value in values)
            {
                if (value is bool)
                {
                    andTrue = andTrue && (bool)value;
                }
            }

            return andTrue;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(AndTrueToTrueMultiConverter)} can only be used in OneWay bindings");
        }
    }
}
