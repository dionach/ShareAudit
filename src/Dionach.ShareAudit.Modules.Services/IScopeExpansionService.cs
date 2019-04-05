using System.Collections.Generic;
using System.Net;

namespace Dionach.ShareAudit.Modules.Services
{
    public interface IScopeExpansionService
    {
        IEnumerable<(string ipAddress, string fqdn)> ExpandScope(string scopeText, bool doNotExpandNamesToIPs);
    }
}
