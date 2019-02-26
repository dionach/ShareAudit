using Dionach.ShareAudit.Modules.Services.Interop;
using System;
using System.ComponentModel;

namespace Dionach.ShareAudit.Modules.Services
{
    public class NetUseConnection : IDisposable
    {
        private readonly string _networkName;

        private bool _disposedValue = false;

        public NetUseConnection(string host, string username, string domain, string password)
        {
            const int ERROR_SESSION_CREDENTIAL_CONFLICT = 1219;
            const int ERROR_SUCCESS = 0;
            const int ERROR_NETWORK_PATH_NOT_FOUND = 53;

            _networkName = "\\\\" + host;
            var netResource = new NetResource()
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Any,
                DisplayType = ResourceDisplaytype.Generic,
                RemoteName = _networkName
            };

            var result = NativeMethods.WNetAddConnection2(
                ref netResource,
                password,
                $"{domain}\\{username}",
                0);

            // If there is already a net connection, cancel it and try again
            if (result == ERROR_SESSION_CREDENTIAL_CONFLICT)
            {
                NativeMethods.WNetCancelConnection2(_networkName, 0, true);
                result = NativeMethods.WNetAddConnection2(
                    ref netResource,
                    password,
                    $"{domain}\\{username}",
                    0);
            }

            if (!(result == ERROR_SUCCESS || result == ERROR_SESSION_CREDENTIAL_CONFLICT || result == ERROR_NETWORK_PATH_NOT_FOUND))
            {
                throw new Win32Exception((int)result);
            }
        }

        ~NetUseConnection()
        {
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    NativeMethods.WNetCancelConnection2(_networkName, 0, true);
                }

                _disposedValue = true;
            }
        }
    }
}
