using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Dionach.ShareAudit.Model
{
    public class Host : INotifyPropertyChanged
    {
        private bool _accessible;
        private string _fqdn;
        private string _ipAddress;
        private ObservableCollection<Share> _shares = new ObservableCollection<Share>();
        private HostState _state = HostState.New;

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

        public string Fqdn
        {
            get => _fqdn;
            set
            {
                if (value == _fqdn)
                {
                    return;
                }

                _fqdn = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Name));
            }
        }

        public string IPAddress
        {
            get => _ipAddress;
            set
            {
                if (ReferenceEquals(value, _ipAddress))
                {
                    return;
                }

                _ipAddress = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Name => string.IsNullOrEmpty(Fqdn) ? IPAddress : Fqdn;

        public ObservableCollection<Share> Shares
        {
            get => _shares;
            set
            {
                if (ReferenceEquals(value, _shares))
                {
                    return;
                }

                _shares = value;
                _shares.CollectionChanged += (s, e) =>
                {
                    foreach (Share share in e.NewItems)
                    {
                        share.PropertyChanged += (ss, se) =>
                        {
                            if (se.PropertyName.Equals(nameof(Share.Accessible)))
                            {
                                Accessible = Shares.Any(x => x.Accessible);
                            }
                        };
                    }
                };
                OnPropertyChanged();
            }
        }

        public HostState State
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
