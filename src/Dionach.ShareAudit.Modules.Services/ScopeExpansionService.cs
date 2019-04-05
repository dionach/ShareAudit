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

        public IEnumerable<(string ipAddress, string fqdn)> ExpandScope(string scopeText, bool doNotExpandNamesToIPs)
        {
            scopeText = scopeText ?? throw new ArgumentNullException(nameof(scopeText));
            scopeText = scopeText.Replace(" ", string.Empty);
            scopeText = scopeText.Replace("\t", string.Empty);

            var lines = scopeText.Split(",;\r\n".ToCharArray());

            var scope = new HashSet<(string ipAddress, string fqdn)>();

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

                                scope.Add((ipAddress.ToString(), ipAddress.AddressFamily == AddressFamily.InterNetworkV6 ? $"{ipAddress.ToString().Replace(':', '-')}.ipv6-literal.net" : string.Empty));
                            }
                        }
                    }
                    else if (IPAddress.TryParse(line, out var ipAddress))
                    {
                        scope.Add((ipAddress.ToString(), ipAddress.AddressFamily == AddressFamily.InterNetworkV6 ? $"{ipAddress.ToString().Replace(':', '-')}.ipv6-literal.net" : string.Empty));
                    }
                    else
                    {
                        if (doNotExpandNamesToIPs)
                        {
                            scope.Add((string.Empty, line));
                        }
                        else if (_dnsUtilitiesService.TryResolveHost(line, out var ipAddresses))
                        {
                            foreach (var address in ipAddresses)
                            {
                                scope.Add((address.ToString(), address.AddressFamily == AddressFamily.InterNetworkV6 ? $"{address.ToString().Replace(':', '-')}.ipv6-literal.net" : string.Empty));
                            }
                        }
                    }
                }
            }

            return scope;
        }
    }
}
