using Dionach.ShareAudit.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Dionach.ShareAudit.Modules.Services
{
    public class FileSystemOperationService : IFileSystemOperationService
    {
        private readonly ISidUtilitiesService _sidUtilitiesService;

        public FileSystemOperationService(ISidUtilitiesService sidUtilitiesService)
        {
            _sidUtilitiesService = sidUtilitiesService ?? throw new ArgumentNullException(nameof(sidUtilitiesService));
        }

        public IEnumerable<AccessRuleEntry> EnumerateAccessRules(string path)
        {
            FileAttributes attributes = FileAttributes.Normal;
            try
            {
                attributes = File.GetAttributes(path);
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (FileNotFoundException)
            {
            }
            catch (IOException)
            {
            }

            AuthorizationRuleCollection authorizationRuleCollection = null;

            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                try
                {
                    authorizationRuleCollection = Directory.GetAccessControl(path).GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
            else
            {
                try
                {
                    authorizationRuleCollection = File.GetAccessControl(path).GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
                }
                catch (InvalidOperationException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            if (authorizationRuleCollection != null)
            {
                foreach (AccessRule authorizationRule in authorizationRuleCollection)
                {
                    string identity = _sidUtilitiesService.SidStringToAccountName(path.TrimStart("\\\\".ToCharArray()).Split('\\')[0], authorizationRule.IdentityReference.Value);
                    string rights = (authorizationRule as FileSystemAccessRule).FileSystemRights.ToString();

                    // Handle rights that .NET decided not to include in the enum http://stackoverflow.com/questions/9694834/encountering-a-filesystemrights-value-that-isnt-defined-in-enumeration
                    switch (rights)
                    {
                        case "268435456":
                            rights = "FullControl";
                            break;

                        case "-536805376":
                            rights = "Modify, Synchronize";
                            break;

                        case "-1610612736":
                            rights = "ReadAndExecute, Synchronize";
                            break;
                    }

                    yield return new AccessRuleEntry
                    {
                        Identity = identity,
                        Rights = rights,
                        Type = authorizationRule.AccessControlType,
                        Inherited = authorizationRule.IsInherited,
                    };
                }
            }
        }

        public IEnumerable<DirectoryEntry> EnumerateDirectories(string path)
        {
            try
            {
                return Directory.EnumerateDirectories(path).Select(x => new DirectoryInfo(x)).Select(x => new DirectoryEntry
                {
                    Name = x.Name,
                    FullName = x.FullName
                });
            }
            catch (IOException)
            {
                return Enumerable.Empty<DirectoryEntry>();
            }
            catch (UnauthorizedAccessException)
            {
                return Enumerable.Empty<DirectoryEntry>();
            }
        }

        public IEnumerable<FileEntry> EnumerateFiles(string path)
        {
            try
            {
                return Directory.EnumerateFiles(path).Select(x => new FileInfo(x)).Select(x => new FileEntry
                {
                    Name = x.Name,
                    FullName = x.FullName
                });
            }
            catch (IOException)
            {
                return Enumerable.Empty<FileEntry>();
            }
            catch (UnauthorizedAccessException)
            {
                return Enumerable.Empty<FileEntry>();
            }
        }

        public EffectiveAccess GetEffectiveAccess(string path)
        {
            var effectiveAccess = new EffectiveAccess { Read = true, Write = true };
            bool isDirectory;

            try
            {
                isDirectory = File.GetAttributes(path).HasFlag(FileAttributes.Directory);
            }
            catch (IOException)
            {
                return new EffectiveAccess { Read = false, Write = false };
            }
            catch (UnauthorizedAccessException)
            {
                return new EffectiveAccess { Read = false, Write = false };
            }

            if (isDirectory)
            {
                try
                {
                    Directory.GetFileSystemEntries(path);
                }
                catch (IOException)
                {
                    effectiveAccess.Read = false;
                }
                catch (UnauthorizedAccessException)
                {
                    effectiveAccess.Read = false;
                }

                if (effectiveAccess.Read)
                {
                    // create a FileSecurity object allowing full control so that we can be sure we
                    // have permissions to delete the file
                    var fileSecurity = new FileSecurity();
                    fileSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, AccessControlType.Allow));

                    try
                    {
                        // create the file in a using statement to allow the FileStream, which
                        // implements IDisposable, to be closed on completion
                        using (File.Create(path + "\\write-test.shareaudit", 1, FileOptions.DeleteOnClose, fileSecurity))
                        {
                        }
                    }
                    catch (IOException)
                    {
                        effectiveAccess.Write = false;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        effectiveAccess.Write = false;
                    }
                }
            }
            else
            {
                try
                {
                    using (File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                    }
                }
                catch (IOException)
                {
                    effectiveAccess.Read = false;
                }
                catch (UnauthorizedAccessException)
                {
                    effectiveAccess.Read = false;
                }

                if (effectiveAccess.Read)
                {
                    try
                    {
                        using (File.Open(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                        {
                        }
                    }
                    catch (IOException)
                    {
                        effectiveAccess.Write = false;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        effectiveAccess.Write = false;
                    }
                }
            }

            return effectiveAccess;
        }

        public string HeadFile(string path, int bytes)
        {
            try
            {
                var bufferSize = Math.Min(bytes, new FileInfo(path).Length);
                var buffer = new byte[bufferSize];
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    fs.Read(buffer, 0, buffer.Length);
                    return Convert.ToBase64String(buffer);
                }
            }
            catch (IOException)
            {
                return default;
            }
            catch (UnauthorizedAccessException)
            {
                return default;
            }
        }
    }
}
