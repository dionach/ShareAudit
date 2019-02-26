using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;

namespace Dionach.ShareAudit.Model
{
    public class AccessRuleEntry : INotifyPropertyChanged
    {
        private string _identity;
        private bool _inherited;
        private string _rights;
        private AccessControlType _type;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Identity
        {
            get => _identity;
            set
            {
                if (value == _identity)
                {
                    return;
                }

                _identity = value;
                OnPropertyChanged();
            }
        }

        public bool Inherited
        {
            get => _inherited;
            set
            {
                if (value == _inherited)
                {
                    return;
                }

                _inherited = value;
                OnPropertyChanged();
            }
        }

        public string Rights
        {
            get => _rights;
            set
            {
                if (value == _rights)
                {
                    return;
                }

                _rights = value;
                OnPropertyChanged();
            }
        }

        public AccessControlType Type
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
