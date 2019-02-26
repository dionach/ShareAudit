using Dionach.ShareAudit.Modules.Services;
using Dionach.ShareAudit.Modules.UserInterface.Views;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;

namespace Dionach.ShareAudit.Modules.UserInterface.ViewModels
{
    public class WelcomeViewModel : BindableBase
    {
        private readonly IFileSystemStoreService _fileSystemStoreService;
        private readonly IRegionManager _regionManager;
        private bool _isBusy;

        public WelcomeViewModel(IRegionManager regionManager, IFileSystemStoreService fileSystemStoreService)
        {
            _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
            _fileSystemStoreService = fileSystemStoreService ?? throw new ArgumentNullException(nameof(fileSystemStoreService));

            Banner = "/Dionach.ShareAudit.Modules.UserInterface;component/Images/Banner.png";
            New = new DelegateCommand(OnNew, CanNew).ObservesProperty(() => IsBusy);
            Load = new DelegateCommand(OnLoad, CanLoad).ObservesProperty(() => IsBusy);
            Help = new DelegateCommand(OnHelp, CanHelp).ObservesProperty(() => IsBusy);
        }

        public string Banner { get; }

        public DelegateCommand Help { get; }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public DelegateCommand Load { get; }

        public DelegateCommand New { get; }

        private bool CanHelp() => !IsBusy;

        private bool CanLoad() => !IsBusy;

        private bool CanNew() => !IsBusy;

        private void OnHelp() => _regionManager.RequestNavigate("ContentRegion", nameof(HelpView));

        private void OnLoad()
        {
            IsBusy = true;

            var dialog = new OpenFileDialog
            {
                Filter = _fileSystemStoreService.ShareAuditFilter,
            };

            if (dialog.ShowDialog() == true)
            {
                var parameters = new NavigationParameters
                {
                    { nameof(ConfigurationViewModel.ProjectPath), dialog.FileName }
                };

                _regionManager.RequestNavigate("ContentRegion", nameof(ConfigurationView), parameters);
            }

            IsBusy = false;
        }

        private async void OnNew()
        {
            IsBusy = true;

            var dialog = new SaveFileDialog
            {
                Filter = _fileSystemStoreService.ShareAuditFilter,
                FileName = _fileSystemStoreService.ShareAuditDefaultFilename
            };

            if (dialog.ShowDialog() == true)
            {
                await _fileSystemStoreService.CreateProjectAsync(dialog.FileName);

                var parameters = new NavigationParameters
                {
                    { nameof(ConfigurationViewModel.ProjectPath), dialog.FileName }
                };

                _regionManager.RequestNavigate("ContentRegion", nameof(ConfigurationView), parameters);
            }

            IsBusy = false;
        }
    }
}
