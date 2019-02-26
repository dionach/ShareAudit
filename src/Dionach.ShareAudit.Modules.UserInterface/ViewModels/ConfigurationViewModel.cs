using Dionach.ShareAudit.Model;
using Dionach.ShareAudit.Modules.Services;
using Dionach.ShareAudit.Modules.UserInterface.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;

namespace Dionach.ShareAudit.Modules.UserInterface.ViewModels
{
    public class ConfigurationViewModel : BindableBase, INavigationAware
    {
        private readonly ICredentialsValidationService _credentialsValidationService;
        private readonly IFileSystemStoreService _fileSystemStoreService;
        private readonly IRegionManager _regionManager;
        private readonly IScopeNormalizationService _scopeNormalizationService;
        private readonly IScopeValidationService _scopeValidationService;
        private string _errorMessageForCredentials;
        private string _errorMessageForDomain;
        private string _errorMessageForPassword;
        private string _errorMessageForScope;
        private string _errorMessageForUsername;
        private bool _isBusy;
        private bool _isProjectUnconfigured;
        private Project _project;
        private string _projectPath;

        public ConfigurationViewModel(
            IRegionManager regionManager,
            IFileSystemStoreService fileSystemStoreService,
            ICredentialsValidationService credentialsValidationService,
            IScopeValidationService scopeValidationService,
            IScopeNormalizationService scopeNormalizationService)
        {
            _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
            _fileSystemStoreService = fileSystemStoreService ?? throw new ArgumentNullException(nameof(fileSystemStoreService));
            _credentialsValidationService = credentialsValidationService ?? throw new ArgumentNullException(nameof(credentialsValidationService));
            _scopeValidationService = scopeValidationService ?? throw new ArgumentNullException(nameof(scopeValidationService));
            _scopeNormalizationService = scopeNormalizationService ?? throw new ArgumentNullException(nameof(scopeNormalizationService));

            Cancel = new DelegateCommand(OnCancel, CanCancel).ObservesProperty(() => IsBusy);
            Next = new DelegateCommand(OnNext, CanNext).ObservesProperty(() => IsBusy);
            Import = new DelegateCommand(OnImport, CanImport).ObservesProperty(() => IsBusy).ObservesProperty(() => Project.Configuration.Scope);
            UseCurrentCredentials = new DelegateCommand(OnUseCurrentCredentials, CanUseCurrentCredentials).ObservesProperty(() => IsBusy);
        }

        public DelegateCommand Cancel { get; }

        public string ErrorMessageForCredentials
        {
            get => _errorMessageForCredentials;
            set => SetProperty(ref _errorMessageForCredentials, value);
        }

        public string ErrorMessageForDomain
        {
            get => _errorMessageForDomain;
            set => SetProperty(ref _errorMessageForDomain, value);
        }

        public string ErrorMessageForPassword
        {
            get => _errorMessageForPassword;
            set => SetProperty(ref _errorMessageForPassword, value);
        }

        public string ErrorMessageForScope
        {
            get => _errorMessageForScope;
            set => SetProperty(ref _errorMessageForScope, value);
        }

        public string ErrorMessageForUsername
        {
            get => _errorMessageForUsername;
            set => SetProperty(ref _errorMessageForUsername, value);
        }

        public DelegateCommand Import { get; }

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        public bool IsProjectUnconfigured
        {
            get => _isProjectUnconfigured;
            private set => SetProperty(ref _isProjectUnconfigured, value);
        }

        public DelegateCommand Next { get; }

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

        public DelegateCommand UseCurrentCredentials { get; }

        public bool IsNavigationTarget(NavigationContext navigationContext) => false;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            ProjectPath = navigationContext.Parameters.GetValue<string>(nameof(ProjectPath));
            if (navigationContext.Parameters.ContainsKey(nameof(Project)))
            {
                Project = navigationContext.Parameters.GetValue<Project>(nameof(Project));
            }
            else
            {
                Project = await _fileSystemStoreService.LoadProjectAsync(ProjectPath);
            }

            IsProjectUnconfigured = Project.State == ProjectState.New;
        }

        private bool CanCancel()
        {
            return !IsBusy;
        }

        private bool CanImport()
        {
            return !IsBusy && string.IsNullOrEmpty(Project?.Configuration?.Scope);
        }

        private bool CanNext()
        {
            return !IsBusy;
        }

        private bool CanUseCurrentCredentials() => !IsBusy;

        private void OnCancel() => _regionManager.RequestNavigate("ContentRegion", nameof(WelcomeView));

        private async void OnImport()
        {
            IsBusy = true;

            ErrorMessageForDomain = string.Empty;
            ErrorMessageForUsername = string.Empty;
            ErrorMessageForPassword = string.Empty;
            ErrorMessageForCredentials = string.Empty;

#pragma warning disable IDE0042 // Deconstruct variable declaration
            var domainValidationResult = await _credentialsValidationService.ValidateDomainAsync(Project.Configuration.Credentials.Domain);
            ErrorMessageForDomain = domainValidationResult.errorMessage;

            var usernameValidationResult = await _credentialsValidationService.ValidateUsernameAsync(Project.Configuration.Credentials.Username);
            ErrorMessageForUsername = usernameValidationResult.errorMessage;

            var passwordValidationResult = (isValid: true, errorMessage: string.Empty);
            if (!Project.Configuration.Credentials.UseCurrentCredentials)
            {
                passwordValidationResult = await _credentialsValidationService.ValidatePasswordAsync(Project.Configuration.Credentials.Password);
                ErrorMessageForPassword = passwordValidationResult.errorMessage;
            }

            var credentialsValidationResult = (isValid: true, errorMessage: string.Empty);
#pragma warning restore IDE0042 // Deconstruct variable declaration
            if (!Project.Configuration.Credentials.UseCurrentCredentials && domainValidationResult.isValid && usernameValidationResult.isValid && passwordValidationResult.isValid)
            {
                credentialsValidationResult = await _credentialsValidationService.ValidateCredentialsAsync(Project.Configuration.Credentials);
                ErrorMessageForCredentials = credentialsValidationResult.errorMessage;
            }

            if (credentialsValidationResult.isValid)
            {
                await _fileSystemStoreService.SaveProjectAsync(Project, ProjectPath);

                var parameters = new NavigationParameters
                {
                    { nameof(ImportScopeViewModel.ProjectPath), ProjectPath },
                    { nameof(ImportScopeViewModel.Project), Project }
                };

                _regionManager.RequestNavigate("ContentRegion", nameof(ImportScopeView), parameters);
            }

            IsBusy = false;
        }

        private async void OnNext()
        {
            IsBusy = true;

            ErrorMessageForDomain = string.Empty;
            ErrorMessageForUsername = string.Empty;
            ErrorMessageForPassword = string.Empty;
            ErrorMessageForCredentials = string.Empty;
            ErrorMessageForScope = string.Empty;

            var credentialsValidationResult = (isValid: true, errorMessage: string.Empty);
            if (!Project.Configuration.IsReadOnly && !Project.Configuration.Credentials.UseCurrentCredentials)
            {
#pragma warning disable IDE0042 // Deconstruct variable declaration
                var domainValidationResult = await _credentialsValidationService.ValidateDomainAsync(Project.Configuration.Credentials.Domain);
                ErrorMessageForDomain = domainValidationResult.errorMessage;

                var usernameValidationResult = await _credentialsValidationService.ValidateUsernameAsync(Project.Configuration.Credentials.Username);
                ErrorMessageForUsername = usernameValidationResult.errorMessage;

                var passwordValidationResult = (isValid: true, errorMessage: string.Empty);
                if (!Project.Configuration.Credentials.UseCurrentCredentials)
                {
                    passwordValidationResult = await _credentialsValidationService.ValidatePasswordAsync(Project.Configuration.Credentials.Password);
                    ErrorMessageForPassword = passwordValidationResult.errorMessage;
                }

                if (domainValidationResult.isValid && usernameValidationResult.isValid && passwordValidationResult.isValid)
                {
                    credentialsValidationResult = await _credentialsValidationService.ValidateCredentialsAsync(Project.Configuration.Credentials);
                    ErrorMessageForCredentials = credentialsValidationResult.errorMessage;
                }
            }

            var scopeValidationResult = await _scopeValidationService.ValidateScopeAsync(Project.Configuration.Scope);
#pragma warning restore IDE0042 // Deconstruct variable declaration
            ErrorMessageForScope = scopeValidationResult.errorMessage;

            if (scopeValidationResult.isValid)
            {
                Project.Configuration.Scope = await _scopeNormalizationService.NormalizeScopeAsync(Project.Configuration.Scope);
            }

            if ((!Project.Configuration.IsReadOnly && credentialsValidationResult.isValid && scopeValidationResult.isValid) || (Project.Configuration.IsReadOnly && scopeValidationResult.isValid))
            {
                if (Project.State < ProjectState.Configured)
                {
                    Project.State = ProjectState.Configured;
                }

                await _fileSystemStoreService.SaveProjectAsync(Project, ProjectPath);

                var parameters = new NavigationParameters
                {
                    { nameof(ImportScopeViewModel.ProjectPath), ProjectPath },
                    { nameof(ImportScopeViewModel.Project), Project }
                };

                _regionManager.RequestNavigate("ContentRegion", nameof(AuditView), parameters);
            }

            IsBusy = false;
        }

        private async void OnUseCurrentCredentials()
        {
            IsBusy = true;

            if (Project.Configuration.Credentials.UseCurrentCredentials)
            {
                (var domain, var username) = await _credentialsValidationService.GetCurrentUserInformation();
                Project.Configuration.Credentials.Domain = domain;
                Project.Configuration.Credentials.Username = username;
                Project.Configuration.Credentials.Password = string.Empty;
                Project.Configuration.UseAlternateAuthenticationMethod = false;
            }
            else
            {
                Project.Configuration.Credentials.Domain = string.Empty;
                Project.Configuration.Credentials.Username = string.Empty;
                Project.Configuration.Credentials.Password = string.Empty;
                Project.Configuration.UseAlternateAuthenticationMethod = true;
            }

            IsBusy = false;
        }
    }
}
