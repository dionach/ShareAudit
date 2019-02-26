using Dionach.ShareAudit.Model;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Dionach.ShareAudit.Modules.UserInterface.Converters
{
    [ValueConversion(typeof(ShareTypes), typeof(ImageSource))]
    public sealed class ShareTypeToIconSourceConverter : IValueConverter
    {
        public static ShareTypeToIconSourceConverter Default { get; } = new ShareTypeToIconSourceConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is ShareTypes))
            {
                throw new NotSupportedException($"{nameof(ShareTypeToIconSourceConverter)} can only be used with {nameof(ShareTypes)} values");
            }

            switch ((ShareTypes)value)
            {
                case ShareTypes.Disktree:
                    return "/Dionach.ShareAudit.Modules.UserInterface;component/Images/imageres_4.ico";
                case ShareTypes.Device:
                    return "/Dionach.ShareAudit.Modules.UserInterface;component/Images/imageres_32.ico";
                case ShareTypes.PrintQueue:
                    return "/Dionach.ShareAudit.Modules.UserInterface;component/Images/imageres_51.ico";
                case ShareTypes.Special:
                    return "/Dionach.ShareAudit.Modules.UserInterface;component/Images/imageres_78.ico";

                default:
                    return Binding.DoNothing;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(ShareTypeToIconSourceConverter)} can only be used in OneWay bindings");
        }
    }
}
