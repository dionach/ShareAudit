using Dionach.ShareAudit.Model;
using System.Threading.Tasks;

namespace Dionach.ShareAudit.Modules.Services
{
    public interface IImportScopeFromActiveDirectoryService
    {
        Task<string> Import(string domain, string username, string password, ImportComputerType importComputerType);

        Task<string> Import(string domain, ImportComputerType importComputerType);
    }
}
