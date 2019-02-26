using Dionach.ShareAudit.Model;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Threading.Tasks;

namespace Dionach.ShareAudit.Modules.Services
{
    public class ImportScopeFromActiveDirectoryService : IImportScopeFromActiveDirectoryService
    {
        public async Task<string> Import(string domain, string username, string password, ImportComputerType importComputerType)
        {
            return await Task.Run(() =>
            {
                var computerNames = new List<string>();

                using (var directoryEntry = new System.DirectoryServices.DirectoryEntry($"LDAP://{domain}", username, password))
                {
                    using (var directorySearcher = new DirectorySearcher(directoryEntry))
                    {
                        switch (importComputerType)
                        {
                            case ImportComputerType.Servers:
                                directorySearcher.Filter = "(&(objectClass=computer)(operatingSystem=Windows Server*))";
                                break;

                            case ImportComputerType.Workstations:
                                directorySearcher.Filter = "(&(objectClass=computer)(|(operatingSystem=Windows XP*)(operatingSystem=Windows Vista*)(operatingSystem=Windows 7*)(operatingSystem=Windows 8*)(operatingSystem=Windows 10*)))";
                                break;

                            case ImportComputerType.All:
                                directorySearcher.Filter = "(objectClass=computer)";
                                break;
                        }

                        directorySearcher.SizeLimit = 0;
                        directorySearcher.PageSize = 250;
                        directorySearcher.PropertiesToLoad.Add("dNSHostName");

                        using (var searchResultCollection = directorySearcher.FindAll())
                        {
                            foreach (SearchResult searchResult in searchResultCollection)
                            {
                                if (searchResult.Properties["dNSHostName"].Count > 0)
                                {
                                    computerNames.Add(searchResult.Properties["dNSHostName"][0] as string);
                                }
                            }
                        }
                    }
                }
                return string.Join(", ", computerNames);
            });
        }

        public async Task<string> Import(string domain, ImportComputerType importComputerType)
        {
            return await Task.Run(() =>
            {
                var computerNames = new List<string>();

                using (var directoryEntry = new System.DirectoryServices.DirectoryEntry($"LDAP://{domain}"))
                {
                    using (var directorySearcher = new DirectorySearcher(directoryEntry))
                    {
                        switch (importComputerType)
                        {
                            case ImportComputerType.Servers:
                                directorySearcher.Filter = "(&(objectClass=computer)(operatingSystem=Windows Server*))";
                                break;

                            case ImportComputerType.Workstations:
                                directorySearcher.Filter = "(&(objectClass=computer)(|(operatingSystem=Windows XP*)(operatingSystem=Windows Vista*)(operatingSystem=Windows 7*)(operatingSystem=Windows 8*)(operatingSystem=Windows 10*)))";
                                break;

                            case ImportComputerType.All:
                                directorySearcher.Filter = "(objectClass=computer)";
                                break;
                        }

                        directorySearcher.SizeLimit = 0;
                        directorySearcher.PageSize = 250;
                        directorySearcher.PropertiesToLoad.Add("dNSHostName");

                        using (var searchResultCollection = directorySearcher.FindAll())
                        {
                            foreach (SearchResult searchResult in searchResultCollection)
                            {
                                if (searchResult.Properties["dNSHostName"].Count > 0)
                                {
                                    computerNames.Add(searchResult.Properties["dNSHostName"][0] as string);
                                }
                            }
                        }
                    }
                }
                return string.Join(", ", computerNames);
            });
        }
    }
}
