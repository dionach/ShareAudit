using Dionach.ShareAudit.Model;
using System.Threading.Tasks;

namespace Dionach.ShareAudit.Modules.Services
{
    public interface ICredentialsValidationService
    {
        Task<(string domain, string username)> GetCurrentUserInformation();

        Task<(bool isValid, string errorMessage)> ValidateCredentialsAsync(Credentials credentials);

        Task<(bool isValid, string errorMessage)> ValidateDomainAsync(string domain);

        Task<(bool isValid, string errorMessage)> ValidatePasswordAsync(string password);

        Task<(bool isValid, string errorMessage)> ValidateUsernameAsync(string username);
    }
}
