using Prism.Mvvm;

namespace Dionach.ShareAudit.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = $"Share Audit - {typeof(MainWindowViewModel).Assembly.GetName().Version}";

        public MainWindowViewModel()
        {
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}
