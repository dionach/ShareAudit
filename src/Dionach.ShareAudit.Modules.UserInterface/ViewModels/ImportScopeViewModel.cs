using Dionach.ShareAudit.Model;
using Dionach.ShareAudit.Modules.Services;
using Dionach.ShareAudit.Modules.UserInterface.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;

namespace Dionach.ShareAudit.Modules.UserInterface.ViewModels
{
    public class ImportScopeViewModel : BindableBase, INavigationAware
    {
        private readonly IFileSystemStoreService _fileSystemStoreService;
        private readonly IImportScopeFromActiveDirectoryService _importScopeFromActiveDirectoryService;
        private readonly IRegionManager _regionManager;
        private string _domain;
        private ImportComputerType _importComputerType = ImportComputerType.Servers;
        private bool _isBusy;
        private Project _project;
        private string _projectPath;

        public ImportScopeViewModel(
            IRegionManager regionManager,
            IFileSystemStoreService fileSystemStoreService,
            IImportScopeFromActiveDirectoryService importScopeFromActiveDirectoryService)
        {
            _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
            _fileSystemStoreService = fileSystemStoreService ?? throw new ArgumentNullException(nameof(fileSystemStoreService));
            _importScopeFromActiveDirectoryService = importScopeFromActiveDirectoryService ?? throw new ArgumentNullException(nameof(importScopeFromActiveDirectoryService));

            Cancel = new DelegateCommand(OnCancel, CanCancel).ObservesProperty(() => IsBusy);
            Import = new DelegateCommand(OnImport, CanImport).ObservesProperty(() => IsBusy);
        }

        public DelegateCommand Cancel { get; }

        public string Domain
        {
            get => _domain;
            set => SetProperty(ref _domain, value);
        }

        public DelegateCommand Import { get; }

        public ImportComputerType ImportComputerType
        {
            get => _importComputerType;
            set => SetProperty(ref _importComputerType, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        public Project Project
        {
            get => _project;
            private set => SetProperty(ref _project, value);
        }

        public string ProjectPath
        {
            get => _projectPath;
            private set => SetProperty(ref _projectPath, value);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => false;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ProjectPath = navigationContext.Parameters.GetValue<string>(nameof(ProjectPath));
            Project = navigationContext.Parameters.GetValue<Project>(nameof(Project));
            Domain = Project.Configuration.Credentials.Domain;
        }

        private bool CanCancel() => !IsBusy;

        private bool CanImport() => !IsBusy;

        private void OnCancel()
        {
            var parameters = new NavigationParameters
                {
                    { nameof(ConfigurationViewModel.ProjectPath), ProjectPath },
                    { nameof(ConfigurationViewModel.Project), Project }
                };

            _regionManager.RequestNavigate("ContentRegion", nameof(ConfigurationView), parameters);
        }

        private async void OnImport()
        {
            IsBusy = true;

            if (Project.Configuration.Credentials.UseCurrentCredentials)
            {
                Project.Configuration.Scope = await _importScopeFromActiveDirectoryService.Import(Domain, ImportComputerType);
            }
            else
            {
                Project.Configuration.Scope = await _importScopeFromActiveDirectoryService.Import(Domain, Project.Configuration.Credentials.Username, Project.Configuration.Credentials.Password, ImportComputerType);
            }

            await _fileSystemStoreService.SaveProjectAsync(Project, ProjectPath);

            var parameters = new NavigationParameters
                {
                    { nameof(ConfigurationViewModel.ProjectPath), ProjectPath },
                    { nameof(ConfigurationViewModel.Project), Project }
                };

            _regionManager.RequestNavigate("ContentRegion", nameof(ConfigurationView), parameters);

            IsBusy = false;
        }
    }
}
