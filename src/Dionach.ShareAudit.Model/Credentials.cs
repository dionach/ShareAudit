using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Dionach.ShareAudit.Model
{
    public class Credentials : INotifyPropertyChanged
    {
        private string _domain = string.Empty;
        private string _password = string.Empty;
        private bool _useCurrentCredentials = false;

        private string _username = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Domain
        {
            get => _domain;
            set
            {
                if (value == _domain)
                {
                    return;
                }

                _domain = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public string Password
        {
            get => _password;
            set
            {
                if (value == _password)
                {
                    return;
                }

                _password = value;
                OnPropertyChanged();
            }
        }

        public bool UseCurrentCredentials
        {
            get => _useCurrentCredentials;
            set
            {
                if (value == _useCurrentCredentials)
                {
                    return;
                }

                _useCurrentCredentials = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                if (value == _username)
                {
                    return;
                }

                _username = value;
                OnPropertyChanged();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
