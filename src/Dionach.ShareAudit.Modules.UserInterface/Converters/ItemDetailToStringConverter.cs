using Dionach.ShareAudit.Model;
using System;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Dionach.ShareAudit.Modules.UserInterface.Converters
{
    [ValueConversion(typeof(FileSystemEntry), typeof(string))]
    public sealed class ItemDetailToStringConverter : IValueConverter
    {
        public static ItemDetailToStringConverter Default { get; } = new ItemDetailToStringConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is FileSystemEntry)
            {
                if ((value is FileEntry && (value as FileEntry).State < FileEntryState.Complete) ||
                    (value is IFolderEntry && (value as IFolderEntry).State < FolderEntryState.GettingEffectiveAccess))
                {
                    return string.Empty;
                }

                var fileSystemEntry = value as FileSystemEntry;
                var sb = new StringBuilder();
                sb.AppendLine(fileSystemEntry.FullName);
                sb.AppendLine();
                sb.AppendLine($"Effective Access: {(fileSystemEntry.EffectiveAccess.Write ? "Write" : "Read")}");
                sb.AppendLine();

                var maxIdentity = Math.Max("Identity".Length, fileSystemEntry.AccessRules.Max(x => x.Identity.Length));
                var maxRights = Math.Max("Rights".Length, fileSystemEntry.AccessRules.Max(x => x.Rights.Length));
                var maxType = Math.Max("Type".Length, fileSystemEntry.AccessRules.Max(x => x.Type.ToString().Length));
                var maxInherited = Math.Max("Inherited".Length, fileSystemEntry.AccessRules.Max(x => x.Inherited.ToString().Length));
                sb.AppendLine($"| {"Identity".PadRight(maxIdentity)} | {"Rights".PadRight(maxRights)} | {"Type".PadRight(maxType)} | {"Inherited".PadRight(maxInherited)} |");
                sb.AppendLine($"| {string.Empty.PadRight(maxIdentity, '-')} | {string.Empty.PadRight(maxRights, '-')} | {string.Empty.PadRight(maxType, '-')} | {string.Empty.PadRight(maxInherited, '-')} |");
                foreach (var accessRule in fileSystemEntry.AccessRules)
                {
                    sb.AppendLine($"| {accessRule.Identity.PadRight(maxIdentity)} | {accessRule.Rights.PadRight(maxRights)} | {accessRule.Type.ToString().PadRight(maxType)} | {accessRule.Inherited.ToString().PadRight(maxInherited)} |");
                }

                if (value is FileEntry && fileSystemEntry.EffectiveAccess.Read)
                {
                    sb.AppendLine();
                    sb.AppendLine("Preview:");
                    sb.AppendLine("--------");
                    sb.Append(Encoding.Default.GetString(System.Convert.FromBase64String((value as FileEntry).Head)));
                }

                return sb.ToString();
            }

            return string.Empty;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(ItemDetailToStringConverter)} can only be used in OneWay bindings");
        }
    }
}
