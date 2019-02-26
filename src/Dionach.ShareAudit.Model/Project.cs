using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Dionach.ShareAudit.Model
{
    public class Project : INotifyPropertyChanged
    {
        private Configuration _configuration = new Configuration();
        private ObservableCollection<Host> _hosts = new ObservableCollection<Host>();
        private ProjectState _state = ProjectState.New;

        public event PropertyChangedEventHandler PropertyChanged;

        public Configuration Configuration
        {
            get => _configuration;
            set
            {
                if (ReferenceEquals(value, _configuration))
                {
                    return;
                }

                _configuration = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Host> Hosts
        {
            get => _hosts;
            set
            {
                if (ReferenceEquals(value, _hosts))
                {
                    return;
                }

                _hosts = value;
                OnPropertyChanged();
            }
        }

        public ProjectState State
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
