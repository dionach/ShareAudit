using System.Collections.Generic;
using System.Net;

namespace Dionach.ShareAudit.Modules.Services
{
    public interface IDnsUtilitiesService
    {
        string GetPtrRecord(IPAddress ipAddress);

        bool TryResolveHost(string host, out IEnumerable<IPAddress> ipAddresses);
    }
}
