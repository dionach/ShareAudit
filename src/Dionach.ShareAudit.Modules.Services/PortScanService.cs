using System.Net.Sockets;

namespace Dionach.ShareAudit.Modules.Services
{
    public class PortScanService : IPortScanService
    {
        public bool IsTcpPortOpen(string host, ushort port, int millisecondsTimeout)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(host, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(millisecondsTimeout);
                    if (!success)
                    {
                        return false;
                    }

                    client.EndConnect(result);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
