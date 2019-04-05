using Dionach.ShareAudit.Model;
using SimpleImpersonation;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Dionach.ShareAudit.Modules.Services
{
    public sealed class ShareAuditService : IShareAuditService, IDisposable, INotifyPropertyChanged
    {
        private readonly BackgroundWorker _backgroundWorker;

        private readonly IDnsUtilitiesService _dnsUtilitiesService;

        private readonly IFileSystemOperationService _fileSystemOperationService;

        private readonly ParallelOptions _parallelOptions = new ParallelOptions
        {
#if DEBUG
            //MaxDegreeOfParallelism = 1
#endif
        };

        private readonly IPortScanService _portScanService;
        private readonly IScopeExpansionService _scopeExpansionService;
        private readonly ISmbUtilitiesService _smbUtilitiesService;
        private bool _disposedValue = false;
        private bool _isBusy;
        private Project _project;
        private SynchronizationContext _uiContext;

        public ShareAuditService(
            IScopeExpansionService scopeExpansionService,
            IDnsUtilitiesService dnsUtilitiesService,
            IPortScanService portScanService,
            ISmbUtilitiesService smbUtilitiesService,
            IFileSystemOperationService fileSystemOperationService)
        {
            _scopeExpansionService = scopeExpansionService ?? throw new ArgumentNullException(nameof(scopeExpansionService));
            _dnsUtilitiesService = dnsUtilitiesService ?? throw new ArgumentNullException(nameof(dnsUtilitiesService));
            _portScanService = portScanService ?? throw new ArgumentNullException(nameof(portScanService));
            _smbUtilitiesService = smbUtilitiesService ?? throw new ArgumentNullException(nameof(smbUtilitiesService));
            _fileSystemOperationService = fileSystemOperationService ?? throw new ArgumentNullException(nameof(fileSystemOperationService));

            _backgroundWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };

            _backgroundWorker.DoWork += DoWork;
            _backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;
        }

        ~ShareAuditService()
        {
            Dispose(false);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler Started;

        public event EventHandler Stopped;

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (value == _isBusy)
                {
                    return;
                }

                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void StartAudit(Project project)
        {
            if (!_backgroundWorker.IsBusy)
            {
                _uiContext = SynchronizationContext.Current;
                _project = project;
                _backgroundWorker.RunWorkerAsync();
            }
        }

        public void StopAudit()
        {
            if (_backgroundWorker.IsBusy)
            {
                _backgroundWorker.CancelAsync();
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _backgroundWorker.Dispose();
                }

                _disposedValue = true;
            }
        }

        private void DoAuthenticatedHostWork(Host host)
        {
            if (host.State == HostState.EnumeratingShares && !_backgroundWorker.CancellationPending)
            {
                _uiContext.Send((_) => host.Shares = new ObservableCollection<Share>(), null);

                foreach (var shareInfo1 in _smbUtilitiesService.NetShareEnum(host.Name))
                {
                    if ((shareInfo1.Type & ShareTypes.Ipc) == ShareTypes.Ipc)
                    {
                        continue;
                    }

                    _uiContext.Send(
                        (_) =>
                        {
                            host.Shares.Add(new Share
                            {
                                Name = shareInfo1.NetName,
                                FullName = $"\\\\{host.Name}\\{shareInfo1.NetName}",
                                Type = shareInfo1.Type,
                                Remark = shareInfo1.Remark
                            });
                        }, null);
                }

                _uiContext.Send((_) => host.State = HostState.AuditingShares, null);
            }

            if ((host.State == HostState.AuditingShares || host.State == HostState.NestedAuditingSuspended) && !_backgroundWorker.CancellationPending)
            {
                Parallel.ForEach(host.Shares, _parallelOptions, (share) =>
                {
                    if (!_backgroundWorker.CancellationPending)
                    {
                        if (share.Type.HasFlag(ShareTypes.PrintQueue))
                        {
                            _uiContext.Send((_) => share.State = FolderEntryState.Complete, null);
                        }
                        else
                        {
                            DoFolderEntryWork(share);
                        }
                    }
                });

                if (host.Shares.All(x => x.State == FolderEntryState.Complete))
                {
                    _uiContext.Send((_) => host.State = HostState.Complete, null);
                }
                else if (host.Shares.All(x => x.State == FolderEntryState.Complete || x.State == FolderEntryState.NestedAuditingSuspended))
                {
                    _uiContext.Send((_) => host.State = HostState.NestedAuditingSuspended, null);
                }
            }
        }

        private void DoFileEntryWork(FileEntry fileEntry)
        {
            if (fileEntry.State == FileEntryState.New && !_backgroundWorker.CancellationPending)
            {
                _uiContext.Send((_) => fileEntry.State = FileEntryState.EnumeratingAcls, null);
            }

            if (fileEntry.State == FileEntryState.EnumeratingAcls && !_backgroundWorker.CancellationPending)
            {
                _uiContext.Send((_) => fileEntry.AccessRules = new ObservableCollection<AccessRuleEntry>(), null);

                foreach (var accessRule in _fileSystemOperationService.EnumerateAccessRules(fileEntry.FullName))
                {
                    _uiContext.Send((_) => fileEntry.AccessRules.Add(accessRule), null);
                }

                _uiContext.Send((_) => fileEntry.State = FileEntryState.GettingEffectiveAccess, null);
            }

            if (fileEntry.State == FileEntryState.GettingEffectiveAccess && !_backgroundWorker.CancellationPending)
            {
                var effectiveAccess = _fileSystemOperationService.GetEffectiveAccess(fileEntry.FullName);
                _uiContext.Send((_) => fileEntry.EffectiveAccess = effectiveAccess, null);

                _uiContext.Send((_) => fileEntry.State = FileEntryState.ReadingHead, null);
            }

            if (fileEntry.State == FileEntryState.ReadingHead && !_backgroundWorker.CancellationPending)
            {
                if (fileEntry.EffectiveAccess.Read)
                {
                    var head = _fileSystemOperationService.HeadFile(fileEntry.FullName, 1024);
                    _uiContext.Send((_) => fileEntry.Head = head, null);
                }

                _uiContext.Send((_) => fileEntry.State = FileEntryState.Complete, null);
            }
        }

        private void DoFolderEntryWork(IFolderEntry folderEntry)
        {
            var level = folderEntry.FullName.Trim('\\').Split('\\').Count() - 1;

            if (folderEntry.State == FolderEntryState.New && !_backgroundWorker.CancellationPending)
            {
                _uiContext.Send((_) => folderEntry.State = FolderEntryState.EnumeratingAcls, null);
            }

            if (folderEntry.State == FolderEntryState.EnumeratingAcls && !_backgroundWorker.CancellationPending)
            {
                _uiContext.Send((_) => folderEntry.AccessRules = new ObservableCollection<AccessRuleEntry>(), null);

                foreach (var accessRule in _fileSystemOperationService.EnumerateAccessRules(folderEntry.FullName))
                {
                    _uiContext.Send((_) => folderEntry.AccessRules.Add(accessRule), null);
                }

                _uiContext.Send((_) => folderEntry.State = FolderEntryState.GettingEffectiveAccess, null);
            }

            if (folderEntry.State == FolderEntryState.GettingEffectiveAccess && !_backgroundWorker.CancellationPending)
            {
                var effectiveAccess = _fileSystemOperationService.GetEffectiveAccess(folderEntry.FullName);
                _uiContext.Send((_) => folderEntry.EffectiveAccess = effectiveAccess, null);

                if (level > 1 && folderEntry is DirectoryEntry)
                {
                    _uiContext.Send((_) => folderEntry.State = FolderEntryState.EnumerationSuspended, null);
                }
                else
                {
                    _uiContext.Send((_) => folderEntry.State = FolderEntryState.EnumeratingFilesystemEntries, null);
                }
            }

            if (folderEntry.State == FolderEntryState.EnumeratingFilesystemEntries && !_backgroundWorker.CancellationPending)
            {
                _uiContext.Send((_) => folderEntry.FileSystemEntries = new ObservableCollection<FileSystemEntry>(), null);

                var directories = Enumerable.Empty<string>();

                foreach (var directory in _fileSystemOperationService.EnumerateDirectories(folderEntry.FullName))
                {
                    _uiContext.Send((_) => { folderEntry.FileSystemEntries.Add(directory); }, null);
                }

                foreach (var files in _fileSystemOperationService.EnumerateFiles(folderEntry.FullName))
                {
                    _uiContext.Send((_) => { folderEntry.FileSystemEntries.Add(files); }, null);
                }

                _uiContext.Send((_) => folderEntry.State = FolderEntryState.AuditingFileSystemEntries, null);
            }

            if (folderEntry.State == FolderEntryState.AuditingFileSystemEntries && !_backgroundWorker.CancellationPending)
            {
                Parallel.ForEach(folderEntry.FileSystemEntries, _parallelOptions, (entry) =>
                {
                    if (!_backgroundWorker.CancellationPending)
                    {
                        if (entry is DirectoryEntry)
                        {
                            DoFolderEntryWork(entry as DirectoryEntry);
                        }
                        else if (entry is FileEntry)
                        {
                            DoFileEntryWork(entry as FileEntry);
                        }
                    }
                });

                if (folderEntry.FileSystemEntries.Where(x => x is DirectoryEntry).All(x => (x as DirectoryEntry).State == FolderEntryState.Complete) &&
                    folderEntry.FileSystemEntries.Where(x => x is FileEntry).All(x => (x as FileEntry).State == FileEntryState.Complete))
                {
                    _uiContext.Send((_) => folderEntry.State = FolderEntryState.Complete, null);
                }
                else if (folderEntry.FileSystemEntries.Where(x => x is DirectoryEntry).All(x => (x as DirectoryEntry).State == FolderEntryState.EnumerationSuspended || (x as DirectoryEntry).State == FolderEntryState.Complete) &&
                    folderEntry.FileSystemEntries.Where(x => x is FileEntry).All(x => (x as FileEntry).State == FileEntryState.Complete))
                {
                    _uiContext.Send((_) => folderEntry.State = FolderEntryState.NestedAuditingSuspended, null);
                }
            }

            if (folderEntry.State == FolderEntryState.NestedAuditingSuspended && !_backgroundWorker.CancellationPending)
            {
                Parallel.ForEach(folderEntry.FileSystemEntries, _parallelOptions, (entry) =>
                {
                    if (!_backgroundWorker.CancellationPending)
                    {
                        if (entry is DirectoryEntry)
                        {
                            DoFolderEntryWork(entry as DirectoryEntry);
                        }
                    }
                });

                if (folderEntry.FileSystemEntries.Where(x => x is DirectoryEntry).All(x => (x as DirectoryEntry).State == FolderEntryState.Complete) &&
                    folderEntry.FileSystemEntries.Where(x => x is FileEntry).All(x => (x as FileEntry).State == FileEntryState.Complete))
                {
                    _uiContext.Send((_) => folderEntry.State = FolderEntryState.Complete, null);
                }
            }
        }

        private void DoHostWork(Host host)
        {
            if (host.State == HostState.New && !_backgroundWorker.CancellationPending)
            {
                _uiContext.Send((_) => host.State = HostState.LookingUpPtr, null);
            }

            if (host.State == HostState.LookingUpPtr && !_backgroundWorker.CancellationPending)
            {
                if (!_project.Configuration.DisableReverseDnsLookup && !string.IsNullOrEmpty(host.IPAddress))
                {
                    var ptrRecord = _dnsUtilitiesService.GetPtrRecord(IPAddress.Parse(host.IPAddress));
                    _uiContext.Send((_) => host.Fqdn = ptrRecord, null);
                }

                _uiContext.Send((_) => host.State = HostState.CheckingPorts, null);
            }

            if (host.State == HostState.CheckingPorts && !_backgroundWorker.CancellationPending)
            {
                if (_project.Configuration.DisablePortScan)
                {
                    _uiContext.Send((_) => host.State = HostState.EnumeratingShares, null);
                }
                else
                {
                    var arePortsAccessible = _portScanService.IsTcpPortOpen(host.Name, 445, 1500);
                    if (arePortsAccessible)
                    {
                        _uiContext.Send((_) => host.State = HostState.EnumeratingShares, null);
                    }
                    else
                    {
                        _uiContext.Send((_) => host.State = HostState.Complete, null);
                    }
                }
            }

            if ((host.State == HostState.EnumeratingShares || host.State == HostState.AuditingShares || host.State == HostState.NestedAuditingSuspended) && !_backgroundWorker.CancellationPending)
            {
                if (_project.Configuration.Credentials.UseCurrentCredentials || _project.Configuration.UseAlternateAuthenticationMethod)
                {
                    DoAuthenticatedHostWork(host);
                }
                else
                {
                    using (var netUseConnection = _smbUtilitiesService.CreateNetUseConnection(host.Name, _project.Configuration.Credentials.Username, _project.Configuration.Credentials.Domain, _project.Configuration.Credentials.Password))
                    {
                        DoAuthenticatedHostWork(host);
                    }
                }
            }
        }

        private void DoProjectWork()
        {
            if (_project.State == ProjectState.Configured && !_backgroundWorker.CancellationPending)
            {
                _uiContext.Send((_) => _project.State = ProjectState.ExpandingScope, null);
            }

            if (_project.State == ProjectState.ExpandingScope && !_backgroundWorker.CancellationPending)
            {
                _uiContext.Send((_) => _project.Hosts = new ObservableCollection<Host>(), null);

                Parallel.ForEach(_scopeExpansionService.ExpandScope(_project.Configuration.Scope, _project.Configuration.UseVerbatimScope), _parallelOptions, (item) =>
                {
                    _uiContext.Send(
                        (_) =>
                        {
                            _project.Hosts.Add(new Host
                            {
                                IPAddress = item.ipAddress,
                                Fqdn = item.fqdn
                            });
                        }, null);
                });

                _uiContext.Send((_) => _project.State = ProjectState.AuditingHosts, null);
            }

            if (_project.State == ProjectState.AuditingHosts && !_backgroundWorker.CancellationPending)
            {
                Parallel.ForEach(_project.Hosts, _parallelOptions, (host) =>
                {
                    if (!_backgroundWorker.CancellationPending)
                    {
                        DoHostWork(host);
                    }
                });

                if (_project.Hosts.All(x => x.State == HostState.Complete))
                {
                    _uiContext.Send((_) => _project.State = ProjectState.Complete, null);
                }
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            Started?.Invoke(this, new EventArgs());

            if (!_project.Configuration.Credentials.UseCurrentCredentials && _project.Configuration.UseAlternateAuthenticationMethod)
            {
                var credentials = new UserCredentials(_project.Configuration.Credentials.Domain, _project.Configuration.Credentials.Username, _project.Configuration.Credentials.Password);
                Impersonation.RunAsUser(credentials, LogonType.NewCredentials, () =>
                {
                    DoProjectWork();
                });
            }
            else
            {
                DoProjectWork();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw e.Error;
            }

            Stopped?.Invoke(this, new EventArgs());
        }
    }
}
