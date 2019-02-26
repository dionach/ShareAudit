using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dionach.ShareAudit.Modules.Services
{
    public class ScopeNormalizationService : IScopeNormalizationService
    {
        public async Task<string> NormalizeScopeAsync(string scope)
        {
            return await Task.Run(() =>
            {
                scope = scope ?? throw new ArgumentNullException(nameof(scope));
                scope = scope.Replace(" ", string.Empty);
                scope = scope.Replace("\t", string.Empty);

                var lines = scope.Split(",;\r\n".ToCharArray());

                return string.Join(", ", lines.Where(x => !string.IsNullOrWhiteSpace(x)));
            });
        }
    }
}
