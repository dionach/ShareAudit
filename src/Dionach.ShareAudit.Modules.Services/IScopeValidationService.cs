using System.Threading.Tasks;

namespace Dionach.ShareAudit.Modules.Services
{
    public interface IScopeValidationService
    {
        Task<(bool isValid, string errorMessage)> ValidateScopeAsync(string scope);
    }
}
