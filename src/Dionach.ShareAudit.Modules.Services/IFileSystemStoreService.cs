using Dionach.ShareAudit.Model;
using System.Threading.Tasks;

namespace Dionach.ShareAudit.Modules.Services
{
    public interface IFileSystemStoreService
    {
        string ExportDefaultFilename { get; }

        string ExportFilter { get; }

        string ShareAuditDefaultFilename { get; }

        string ShareAuditFilter { get; }

        Task CreateProjectAsync(string path);

        Task ExportProjectAsync(Project project, string path);

        Task<Project> LoadProjectAsync(string path);

        Task SaveProjectAsync(Project project, string path);
    }
}
