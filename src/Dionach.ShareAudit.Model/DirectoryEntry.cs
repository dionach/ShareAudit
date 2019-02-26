using System.Collections.ObjectModel;

namespace Dionach.ShareAudit.Model
{
    public class DirectoryEntry : FileSystemEntry, IFolderEntry
    {
        private ObservableCollection<FileSystemEntry> _fileSystemEntries = new ObservableCollection<FileSystemEntry>();
        private FolderEntryState _state = FolderEntryState.New;

        public ObservableCollection<FileSystemEntry> FileSystemEntries
        {
            get => _fileSystemEntries;
            set
            {
                if (ReferenceEquals(value, _fileSystemEntries))
                {
                    return;
                }

                _fileSystemEntries = value;
                OnPropertyChanged();
            }
        }

        public FolderEntryState State
        {
            get => _state;
            set
            {
                if (value == _state)
                {
                    return;
                }

                _state = value;
                OnPropertyChanged();
            }
        }
    }
}
