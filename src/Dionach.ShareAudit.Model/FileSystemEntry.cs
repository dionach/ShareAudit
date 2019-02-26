using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Dionach.ShareAudit.Model
{
    [XmlInclude(typeof(DirectoryEntry))]
    [XmlInclude(typeof(FileEntry))]
    public abstract class FileSystemEntry : INotifyPropertyChanged
    {
        private bool _accessible;
        private ObservableCollection<AccessRuleEntry> _accessRules;
        private EffectiveAccess _effectiveAccess = new EffectiveAccess { Read = false, Write = false };
        private string _fullName;

        private string _name;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Accessible
        {
            get => _accessible;
            set
            {
                if (value == _accessible)
                {
                    return;
                }

                _accessible = value;
                OnPropertyChanged();
            }
        }

        public virtual ObservableCollection<AccessRuleEntry> AccessRules
        {
            get => _accessRules;
            set
            {
                if (ReferenceEquals(value, _accessRules))
                {
                    return;
                }

                _accessRules = value;
                _accessRules.CollectionChanged += (s, e) => Accessible = _accessRules.Count > 0;
                OnPropertyChanged();
            }
        }

        public EffectiveAccess EffectiveAccess
        {
            get => _effectiveAccess;
            set
            {
                if (ReferenceEquals(value, _effectiveAccess))
                {
                    return;
                }

                _effectiveAccess = value;
                OnPropertyChanged();
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                if (value == _fullName)
                {
                    return;
                }

                _fullName = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name)
                {
                    return;
                }

                _name = value;
                OnPropertyChanged();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
