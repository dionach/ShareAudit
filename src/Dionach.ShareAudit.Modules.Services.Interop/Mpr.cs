using System.Runtime.InteropServices;

namespace Dionach.ShareAudit.Modules.Services.Interop
{
    internal enum ResourceDisplaytype : int
    {
        Generic = 0x0,
        Domain = 0x01,
        Server = 0x02,
        Share = 0x03,
        File = 0x04,
        Group = 0x05,
        Network = 0x06,
        Root = 0x07,
        Shareadmin = 0x08,
        Directory = 0x09,
        Tree = 0x0a,
        Ndscontainer = 0x0b
    }

    internal enum ResourceScope : int
    {
        Connected = 1,
        GlobalNetwork,
        Remembered,
        Recent,
        Context
    }

    internal enum ResourceType : int
    {
        Any = 0,
        Disk = 1,
        Print = 2,
        Reserved = 8,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NetResource
    {
        internal ResourceScope Scope;
        internal ResourceType ResourceType;
        internal ResourceDisplaytype DisplayType;
        internal int Usage;
        internal string LocalName;
        internal string RemoteName;
        internal string Comment;
        internal string Provider;
    }

    internal partial class NativeMethods
    {
        [DllImport("mpr.dll")]
        internal static extern int WNetAddConnection2(ref NetResource netResource, string password, string username, uint flags);

        [DllImport("mpr.dll")]
        internal static extern int WNetCancelConnection2(string name, int flags, bool force);
    }
}
