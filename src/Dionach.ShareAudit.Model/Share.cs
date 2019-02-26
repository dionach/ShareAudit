using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Dionach.ShareAudit.Model
{
    public class Share : FileSystemEntry, INotifyPropertyChanged, IFolderEntry
    {
        private ObservableCollection<FileSystemEntry> _fileSystemEntries = new ObservableCollection<FileSystemEntry>();
        private string _remark;
        private FolderEntryState _state = FolderEntryState.New;
        private ShareTypes _type;

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

        public string Remark
        {
            get => _remark;
            set
            {
                if (value == _remark)
                {
                    return;
                }

                _remark = value;
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

        public ShareTypes Type
        {
            get => _type;
            set
            {
                if (value == _type)
                {
                    return;
                }

                _type = value;
                OnPropertyChanged();
            }
        }
    }
}
