using Dionach.ShareAudit.Modules.UserInterface.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;

namespace Dionach.ShareAudit.Modules.UserInterface.ViewModels
{
    public class HelpViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public HelpViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));

            Back = new DelegateCommand(OnBack, () => true);
        }

        public DelegateCommand Back { get; }

        private void OnBack() => _regionManager.RequestNavigate("ContentRegion", nameof(WelcomeView));
    }
}
