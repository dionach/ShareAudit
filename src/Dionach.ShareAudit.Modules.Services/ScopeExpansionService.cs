using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Dionach.ShareAudit.Modules.Services
{
    public class ScopeExpansionService : IScopeExpansionService
    {
        private readonly IDnsUtilitiesService _dnsUtilitiesService;

        public ScopeExpansionService(
            IDnsUtilitiesService dnsUtilitiesService)
        {
            _dnsUtilitiesService = dnsUtilitiesService ?? throw new ArgumentNullException(nameof(dnsUtilitiesService));
        }

        public IEnumerable<IPAddress> ExpandScopeToIPAddresses(string scope)
        {
            scope = scope ?? throw new ArgumentNullException(nameof(scope));
            scope = scope.Replace(" ", string.Empty);
            scope = scope.Replace("\t", string.Empty);

            var lines = scope.Split(",;\r\n".ToCharArray());

            var scopeIPAddresses = new HashSet<IPAddress>();

            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (line.Contains("/") && IPNetwork.TryParse(line, out var ipNetwork))
                    {
                        using (var ipAddresses = ipNetwork.ListIPAddress())
                        {
                            foreach (var ipAddress in ipAddresses)
                            {
                                if (ipAddress.AddressFamily == AddressFamily.InterNetwork && (ipAddress.Equals(ipNetwork.Network) || ipAddress.Equals(ipNetwork.Broadcast)))
                                {
                                    continue;
                                }

                                scopeIPAddresses.Add(ipAddress);
                            }
                        }
                    }
                    else if (IPAddress.TryParse(line, out var ipAddress))
                    {
                        scopeIPAddresses.Add(ipAddress);
                    }
                    else if (_dnsUtilitiesService.TryResolveHost(line, out var ipAddresses))
                    {
                        foreach (var address in ipAddresses)
                        {
                            scopeIPAddresses.Add(address);
                        }
                    }
                }
            }

            return scopeIPAddresses;
        }
    }
}
