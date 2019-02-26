using System.Threading.Tasks;

namespace Dionach.ShareAudit.Modules.Services
{
    public interface IScopeNormalizationService
    {
        Task<string> NormalizeScopeAsync(string scope);
    }
}
