using Prism.Ioc;
using Prism.Modularity;

namespace Dionach.ShareAudit.Modules.Services
{
    public class ServicesModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IFileSystemStoreService, FileSystemStoreService>();
            containerRegistry.RegisterSingleton<ICredentialsValidationService, CredentialsValidationService>();
            containerRegistry.RegisterSingleton<IScopeValidationService, ScopeValidationService>();
            containerRegistry.RegisterSingleton<IScopeNormalizationService, ScopeNormalizationService>();
            containerRegistry.RegisterSingleton<IImportScopeFromActiveDirectoryService, ImportScopeFromActiveDirectoryService>();
            containerRegistry.RegisterSingleton<IDnsUtilitiesService, DnsUtilitiesService>();
            containerRegistry.RegisterSingleton<IScopeExpansionService, ScopeExpansionService>();
            containerRegistry.RegisterSingleton<IShareAuditService, ShareAuditService>();
            containerRegistry.RegisterSingleton<IPortScanService, PortScanService>();
            containerRegistry.RegisterSingleton<ISmbUtilitiesService, SmbUtilitiesService>();
            containerRegistry.RegisterSingleton<IFileSystemOperationService, FileSystemOperationService>();
            containerRegistry.RegisterSingleton<ISidUtilitiesService, SidUtilitiesService>();
        }
    }
}
