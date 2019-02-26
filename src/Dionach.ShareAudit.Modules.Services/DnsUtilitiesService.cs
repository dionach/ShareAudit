using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Dionach.ShareAudit.Modules.Services
{
    public class DnsUtilitiesService : IDnsUtilitiesService
    {
        public string GetPtrRecord(IPAddress ipAddress)
        {
            try
            {
                var ptrRecord = Dns.GetHostEntry(ipAddress).HostName;
                if (TryResolveHost(ptrRecord, out var resolvedIPAddresses))
                {
                    if (resolvedIPAddresses.Contains(ipAddress))
                    {
                        return ptrRecord;
                    }
                }
            }
            catch (SocketException)
            {
            }

            return null;
        }

        public bool TryResolveHost(string host, out IEnumerable<IPAddress> ipAddresses)
        {
            host = host ?? throw new ArgumentNullException(nameof(host));

            try
            {
                ipAddresses = Dns.GetHostAddresses(host);
                return true;
            }
            catch (SocketException ex) when (ex.Message == "No such host is known")
            {
                ipAddresses = Array.Empty<IPAddress>();
                return false;
            }
        }
    }
}
