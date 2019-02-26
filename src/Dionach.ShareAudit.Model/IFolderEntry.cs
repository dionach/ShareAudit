using System.Collections.ObjectModel;

namespace Dionach.ShareAudit.Model
{
    public interface IFolderEntry
    {
        ObservableCollection<AccessRuleEntry> AccessRules { get; set; }

        EffectiveAccess EffectiveAccess { get; set; }

        ObservableCollection<FileSystemEntry> FileSystemEntries { get; set; }

        string FullName { get; set; }

        FolderEntryState State { get; set; }
    }
}
