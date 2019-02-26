using System.Threading.Tasks;

namespace Dionach.ShareAudit.Modules.Services
{
    public class ScopeValidationService : IScopeValidationService
    {
        public async Task<(bool isValid, string errorMessage)> ValidateScopeAsync(string scope)
        {
            return await Task.Run(() =>
            {
                var result = (isValid: true, errorMessage: string.Empty);

                if (string.IsNullOrWhiteSpace(scope))
                {
                    result.isValid = false;
                    result.errorMessage = "Scope cannot be empty";
                }

                return result;
            });
        }
    }
}
