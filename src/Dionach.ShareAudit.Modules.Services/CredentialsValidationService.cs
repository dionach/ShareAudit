using Dionach.ShareAudit.Model;
using System;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;

namespace Dionach.ShareAudit.Modules.Services
{
    public class CredentialsValidationService : ICredentialsValidationService
    {
        public async Task<(string domain, string username)> GetCurrentUserInformation()
        {
            return await Task.Run(() =>
            {
                return (domain: Environment.UserDomainName, username: Environment.UserName);
            });
        }

        public async Task<(bool isValid, string errorMessage)> ValidateCredentialsAsync(Credentials credentials)
        {
            return await Task.Run(() =>
            {
                var result = (isValid: true, errorMessage: string.Empty);

                try
                {
                    using (var context = new PrincipalContext(ContextType.Domain, credentials.Domain))
                    {
                        if (!context.ValidateCredentials(credentials.Username, credentials.Password))
                        {
                            result.isValid = false;
                            result.errorMessage = "Domain, user name, or password are incorrect";
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.isValid = false;
                    result.errorMessage = $"Credentials could not be validated: {ex.Message}";
                }

                return result;
            });
        }

        public async Task<(bool isValid, string errorMessage)> ValidateDomainAsync(string domain)
        {
            return await Task.Run(() =>
            {
                var result = (isValid: true, errorMessage: string.Empty);

                if (string.IsNullOrWhiteSpace(domain))
                {
                    result.isValid = false;
                    result.errorMessage = "Domain cannot be empty";
                }

                return result;
            });
        }

        public async Task<(bool isValid, string errorMessage)> ValidatePasswordAsync(string password)
        {
            return await Task.Run(() =>
            {
                var result = (isValid: true, errorMessage: string.Empty);

                if (string.IsNullOrWhiteSpace(password))
                {
                    result.isValid = false;
                    result.errorMessage = "Password cannot be empty";
                }

                return result;
            });
        }

        public async Task<(bool isValid, string errorMessage)> ValidateUsernameAsync(string username)
        {
            return await Task.Run(() =>
            {
                var result = (isValid: true, errorMessage: string.Empty);

                if (string.IsNullOrWhiteSpace(username))
                {
                    result.isValid = false;
                    result.errorMessage = "User name cannot be empty";
                }

                return result;
            });
        }
    }
}
