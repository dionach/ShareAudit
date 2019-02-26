using System.Collections.Generic;
using System.Net;

namespace Dionach.ShareAudit.Modules.Services
{
    public interface IScopeExpansionService
    {
        IEnumerable<IPAddress> ExpandScopeToIPAddresses(string scope);
    }
}
