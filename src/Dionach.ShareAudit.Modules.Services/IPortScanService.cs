namespace Dionach.ShareAudit.Modules.Services
{
    public interface IPortScanService
    {
        bool IsTcpPortOpen(string host, ushort port, int millisecondsTimeout);
    }
}
