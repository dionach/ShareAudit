using Dionach.ShareAudit.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Dionach.ShareAudit.Modules.UserInterface.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public sealed class IsStateInProgressToBoolConverter : IValueConverter
    {
        public static IsStateInProgressToBoolConverter Default { get; } = new IsStateInProgressToBoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is HostState)
            {
                return (HostState)value != HostState.Complete && (HostState)value != HostState.NestedAuditingSuspended;
            }

            if (value is FolderEntryState)
            {
                return (FolderEntryState)value != FolderEntryState.Complete && (FolderEntryState)value != FolderEntryState.EnumerationSuspended && (FolderEntryState)value != FolderEntryState.NestedAuditingSuspended;
            }

            if (value is FileEntryState)
            {
                return (FileEntryState)value != FileEntryState.Complete;
            }

            return Binding.DoNothing;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(IsStateInProgressToBoolConverter)} can only be used in OneWay bindings");
        }
    }
}
